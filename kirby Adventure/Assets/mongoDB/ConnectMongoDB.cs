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
            if (Kirby.instance != null)
            {
                Kirby.instance.OnDamageTaken += OnPlayerDamage;
                Kirby.instance.OnDeadStart += OnPlayerDeath;
            }
            else
            {
                Kirby possible = FindObjectOfType<Kirby>();
                if (possible != null)
                {
                    possible.OnDamageTaken += OnPlayerDamage;
                    possible.OnDeadStart += OnPlayerDeath;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("No se pudo suscribir a los eventos de Kirby: " + ex.Message);
        }
    }

    [Serializable]
    public class PlayerStats
    {
        public int vida = 0;
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
            playerData.vida = Kirby.instance.HP;
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
            playerData.vida = Kirby.instance.HP;
            Debug.Log("Actualizando playerData.vida con el valor actual de Kirby.HP: " + playerData.vida);
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