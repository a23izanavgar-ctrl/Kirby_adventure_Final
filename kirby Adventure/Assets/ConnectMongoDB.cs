using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class ConnectMongoDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    public static IMongoCollection<BsonDocument> usersCollection;

    public PLayerStats playerData;
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

        
        if (playerData == null)
        {
            Debug.LogWarning("playerData era NULL, se inicializa automáticamente");
            playerData = new PLayerStats();
        }
    }

    public class PLayerStats
    {
        public int vida = 0;
    }

    public void enviarDatos()
    {
        if (playerData == null)
        {
            Debug.LogError("playerData sigue siendo NULL, no se pueden enviar datos");
            return;
        }

        document = new BsonDocument
        {
            { "coins", playerData.vida }
        };

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
}