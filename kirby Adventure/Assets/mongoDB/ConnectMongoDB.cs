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
<<<<<<< HEAD
    private Kirby kirby;
    private HUDKirby hud; // Referencia al HUD

    private int absorbCounter = 0; // Contador de absorciones
    private int tiempoDeJuego = 0; // Variable para almacenar el tiempo de juego en segundos
=======
>>>>>>> parent of 9605aca (java sql)

    //1. Montar clase serializada de datos analytics a guardar
    //2. Crear una instancia de esta clase por sesion de partida (pulsar al play)
    //3. Actualizar las variables de la instancia de sesión durante la partida (        Kirby.instance.OnDamageTaken += MetodoQueACtualizaLaVidaDeKirbyEnJSON);
    //4. Enviar la estructura de datos a mongo.
    void Start()
    {
        string connectionString = "mongodb+srv://a25albgilher_db_user:y7I3mVdd9FyoMzar@cluster0.xptrflm.mongodb.net/?appName=Cluster0";

        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("Project0");
            usersCollection = database.GetCollection<BsonDocument>("a25albgilher_db_user");

            Debug.Log("Se ha conectado a la BD");

            // Eliminar el campo histórico "coins" de documentos ya existentes en la colección
            try
            {
                var filter = Builders<BsonDocument>.Filter.Exists("coins");
                var update = Builders<BsonDocument>.Update.Unset("coins");
                var result = usersCollection.UpdateMany(filter, update);
                Debug.Log("Campo 'coins' eliminado de documentos existentes. ModifiedCount=" + result.ModifiedCount);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("No se pudo eliminar el campo 'coins' de documentos existentes: " + ex.Message);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }

        // 2. Crear una instancia de la clase por sesión si es necesario
        if (playerData == null)
        {
            Debug.LogWarning("playerData era NULL, se inicializa automáticamente");
            playerData = new PlayerStats();
        }

        // 3. Suscribirse al evento de daño de Kirby para mantener los datos actualizados durante la partida
        // Intentamos usar el singleton; si es null buscamos la instancia en la escena.
        try
        {
            if (Kirby.instance != null)
            {
                Kirby.instance.OnDamageTaken += OnPlayerDamage;
            }
            else
            {
                Kirby possible = FindObjectOfType<Kirby>();
                if (possible != null)
                    possible.OnDamageTaken += OnPlayerDamage;
            }
        }
        catch (Exception ex)
        {
<<<<<<< HEAD
            Debug.LogError("Error al suscribirse a eventos de Kirby: " + ex.Message);
        }

        // Inicializar referencia al HUD
        hud = FindObjectOfType<HUDKirby>();
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
                .Set("tiempoDeJuego", FormatearTiempoDeJuego(tiempoDeJuego)) // Guardar tiempo formateado en MongoDB
                .Set("score", hud != null ? hud.GetScore() : 0); // Añadir el puntaje al documento

            usersCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

            Debug.Log($"Datos actualizados en MongoDB: Vida = {playerData.vida}, Enemigos absorbidos = {absorbCounter}, Tiempo de juego = {FormatearTiempoDeJuego(tiempoDeJuego)}, Puntaje = {hud?.GetScore() ?? 0}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al actualizar los datos en MongoDB: " + e.Message);
=======
            Debug.LogWarning("No se pudo suscribir a Kirby.OnDamageTaken: " + ex.Message);
>>>>>>> parent of 9605aca (java sql)
        }
    }

    // 1. Clase serializable con los datos de analytics a guardar
    [Serializable]
    public class PlayerStats
    {
        // vida actual del jugador (se actualiza con Kirby.HP)
        public int vida = 0;
        // marca temporal de la sesión o del evento
        public string sessionTimestamp = DateTime.UtcNow.ToString("o");

        // Convierte la estructura a BsonDocument para enviar a Mongo
        public BsonDocument ToBson()
        {
            return new BsonDocument
            {
                { "vida", vida },
                { "sessionTimestamp", sessionTimestamp }
            };
        }
    }

    // Método llamado cuando Kirby recibe daño
    private void OnPlayerDamage()
    {
        if (playerData == null)
            playerData = new PlayerStats();

        if (Kirby.instance != null)
        {
            playerData.vida = Kirby.instance.HP;
            Debug.Log("PlayerStats actualizados: vida=" + playerData.vida);
        }
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

<<<<<<< HEAD
        // Actualizar playerData.vida con el valor actual de Kirby.HP antes de enviar los datos
        if (Kirby.instance != null)
        {
            playerData.vida = Mathf.Max(0, Kirby.instance.HP);
        }

        try
        {
            var filter = Builders<BsonDocument>.Filter.Eq("playerId", playerData.playerId);
            var update = Builders<BsonDocument>.Update
                .Set("vida", playerData.vida)
                .Set("absorbCounter", absorbCounter)
                .Set("tiempoDeJuego", FormatearTiempoDeJuego(tiempoDeJuego))
                .Set("score", hud != null ? hud.GetScore() : 0); // Añadir el puntaje al documento

            usersCollection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

            Debug.Log($"Datos enviados a MongoDB: Vida = {playerData.vida}, Enemigos absorbidos = {absorbCounter}, Tiempo de juego = {FormatearTiempoDeJuego(tiempoDeJuego)}, Puntaje = {hud?.GetScore() ?? 0}");
=======
        document = playerData.ToBson();

        try
        {
            usersCollection.InsertOne(document);
            Debug.Log("¡Datos enviados a MongoDB Atlas!");
>>>>>>> parent of 9605aca (java sql)
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
                Kirby.instance.OnDamageTaken -= OnPlayerDamage;
            else
            {
                Kirby possible = FindObjectOfType<Kirby>();
                if (possible != null)
                    possible.OnDamageTaken -= OnPlayerDamage;
            }
        }
        catch { }
    }
}