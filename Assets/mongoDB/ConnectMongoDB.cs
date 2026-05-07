using System;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class ConnectMongoDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    public static IMongoCollection<BsonDocument> usersCollection;

    public PlayerStats playerData;
    BsonDocument document;
    private Kirby kirby;

    private int absorbCounter = 0; // Contador de absorciones
    private int tiempoDeJuego = 0; // Variable para almacenar el tiempo de juego en segundos

    void Start()
    {
        string connectionString = "mongodb+srv://a25albgilher_db_user:y7I3mVdd9FyoMzar@cluster0.xptrflm.mongodb.net/?appName=Cluster0";

        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("Project0");
            usersCollection = database.GetCollection<BsonDocument>("a25albgilher_db_user");

            Debug.Log("Se ha conectado a la BD");
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }

        // Crear una instancia de la clase por sesión si es necesario
        if (playerData == null)
        {
            Debug.LogWarning("playerData era NULL, se inicializa automáticamente");
            playerData = new PlayerStats();
        }

        // Suscribirse al evento de daño de Kirby para mantener los datos actualizados durante la partida
        try
        {
            // Prefer the singleton instance if it exists, otherwise find any Kirby in the scene
            if (Kirby.instance != null)
            {
                kirby = Kirby.instance;
            }
            else
            {
                kirby = FindObjectOfType<Kirby>();
            }

            if (kirby != null)
            {
                kirby.OnDamageTaken += OnPlayerDamage;
                kirby.OnDeadStart += OnPlayerDeath;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al suscribirse a eventos de Kirby: " + ex.Message);
        }
    }

    void Update()
    {
        // Incrementar el tiempo de juego
        tiempoDeJuego = Mathf.FloorToInt(Time.time);

        // Detectar la tecla Q para incrementar el contador
        if (Input.GetKeyDown(KeyCode.Q))
        {
            absorbCounter++;
            Debug.Log("Kirby ha absorbido. Contador: " + absorbCounter);
            UpdateAbsorbCounterInMongoDB();
        }
    }

    private string FormatearTiempoDeJuego(int tiempoEnSegundos)
    {
        if (tiempoEnSegundos < 60)
        {
            return $"{tiempoEnSegundos} segundos";
        }
        else
        {
            int minutos = tiempoEnSegundos / 60;
            int segundos = tiempoEnSegundos % 60;
            return segundos > 0 ? $"{minutos} minutos y {segundos} segundos" : $"{minutos} minutos";
        }
    }

    private void UpdateAbsorbCounterInMongoDB()
    {
        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("playerId", playerData.playerId);
            var update = Builders<BsonDocument>.Update
                .Set("absorbCounter", absorbCounter)
                .Set("vida", playerData.vida)
                .Set("tiempoDeJuego", FormatearTiempoDeJuego(tiempoDeJuego)); // Guardar tiempo formateado en MongoDB

            usersCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

            Debug.Log($"Datos actualizados en MongoDB: Vida = {playerData.vida}, Enemigos absorbidos = {absorbCounter}, Tiempo de juego = {FormatearTiempoDeJuego(tiempoDeJuego)}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al actualizar los datos en MongoDB: " + e.Message);
        }
    }

    [Serializable]
    public class PlayerStats
    {
        public int vida = 0;
        public string playerId = Guid.NewGuid().ToString();
        public string sessionTimestamp = DateTime.UtcNow.ToString("o");

        public BsonDocument ToBson()
        {
            return new BsonDocument
            {
                { "vida", vida },
                { "sessionTimestamp", sessionTimestamp }
            };
        }
    }

    private void OnPlayerDamage()
    {
        if (playerData == null)
            playerData = new PlayerStats();

        if (Kirby.instance != null)
        {
            playerData.vida = Mathf.Max(0, Kirby.instance.HP); // Asegurar que la vida no sea negativa
            Debug.Log("PlayerStats actualizados: vida=" + playerData.vida);
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Kirby ha muerto. Enviando datos a MongoDB...");
        enviarDatos();
    }

    public void enviarDatos()
    {
        if (playerData == null)
        {
            Debug.LogError("playerData sigue siendo NULL, no se pueden enviar datos");
            return;
        }

        if (usersCollection == null)
        {
            Debug.LogError("Collection de MongoDB no inicializada, no se pueden enviar datos");
            return;
        }

        // Actualizar playerData.vida con el valor actual de Kirby.HP antes de enviar los datos
        if (Kirby.instance != null)
        {
            playerData.vida = Mathf.Max(0, Kirby.instance.HP); // Asegurar que la vida no sea negativa
        }

        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("playerId", playerData.playerId);
            var update = Builders<BsonDocument>.Update
                .Set("vida", playerData.vida)
                .Set("absorbCounter", absorbCounter)
                .Set("tiempoDeJuego", FormatearTiempoDeJuego(tiempoDeJuego));

            usersCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

            Debug.Log($"Datos enviados a MongoDB: Vida = {playerData.vida}, Enemigos absorbidos = {absorbCounter}, Tiempo de juego = {FormatearTiempoDeJuego(tiempoDeJuego)}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al insertar los datos: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        enviarDatos();
    }

    private void OnDestroy()
    {
        // Cancelar la suscripción para evitar referencias colgantes
        try
        {
            if (Kirby.instance != null)
            {
                Kirby.instance.OnDamageTaken -= OnPlayerDamage;
                Kirby.instance.OnDeadStart -= OnPlayerDeath;
            }
            else
            {
                Kirby possible = FindObjectOfType<Kirby>();
                if (possible != null)
                {
                    possible.OnDamageTaken -= OnPlayerDamage;
                    possible.OnDeadStart -= OnPlayerDeath;
                }
            }
        }
        catch { }
    }
}