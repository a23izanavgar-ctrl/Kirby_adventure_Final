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
            Debug.LogWarning("No se pudo suscribir a Kirby.OnDamageTaken: " + ex.Message);
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

        document = playerData.ToBson();

        try
        {
            usersCollection.InsertOne(document);
            Debug.Log("¡Datos enviados a MongoDB Atlas!");
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