using System;
using System.IO;
using UnityEngine;

public class SaveGameData : MonoBehaviour
{
    /** ruta on es guardara el JSON (persistentDataPath funciona en totes les plataformes) */
    private string rutaJSON;

    /** clase serialitzable amb les dades que volem guardar */
    [Serializable]
    public class DadesPartida
    {
        public string playerId;
        public int vida;
        public int absorbCounter;
        public string tiempoDeJuego;
        public string sessionTimestamp;
    }

    void Start()
    {
        /** construim la ruta completa del fitxer */
        rutaJSON = "D:/DUnity/ProjecteFinal_KirbysAdventure/ProjecteJava/kirby_stats.json";
        Debug.Log("Ruta del JSON: " + rutaJSON);
    }

    /** crida aquest metode per guardar les dades al fitxer JSON */
    public void GuardarDades(string playerId, int vida, int absorbCounter, string tiempoDeJuego)
    {
        try
        {
            DadesPartida dades = new DadesPartida();
            dades.playerId = playerId;
            dades.vida = vida;
            dades.absorbCounter = absorbCounter;
            dades.tiempoDeJuego = tiempoDeJuego;
            dades.sessionTimestamp = DateTime.UtcNow.ToString("o");

            /** convertim l'objecte a JSON i l'escrivim al fitxer */
            string json = JsonUtility.ToJson(dades, true);
            File.WriteAllText(rutaJSON, json);

            Debug.Log("Dades guardades al JSON correctament:\n" + json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardant el JSON: " + e.Message);
        }
    }

    /** crida aquest metode per llegir les dades del fitxer JSON */
    public DadesPartida LlegirDades()
    {
        try
        {
            if (!File.Exists(rutaJSON))
            {
                Debug.LogWarning("El fitxer JSON no existeix encara: " + rutaJSON);
                return null;
            }
            string json = File.ReadAllText(rutaJSON);
            DadesPartida dades = JsonUtility.FromJson<DadesPartida>(json);
            Debug.Log("Dades llegides del JSON: vida=" + dades.vida + " absorb=" + dades.absorbCounter);
            return dades;
        }
        catch (Exception e)
        {
            Debug.LogError("Error llegint el JSON: " + e.Message);
            return null;
        }
    }

    /** quan el joc s'acaba, guardem automaticament */
    private void OnApplicationQuit()
    {
        /** agafem les dades del ConnectMongoDB si existeix */
        ConnectMongoDB mongo = FindObjectOfType<ConnectMongoDB>();
        if (mongo != null && mongo.playerData != null)
        {
            int temps = Mathf.FloorToInt(Time.time);
            string tempsFormatat = temps < 60 ? temps + " segons" :
                (temps / 60) + " minuts i " + (temps % 60) + " segons";

            GuardarDades(
                mongo.playerData.playerId,
                mongo.playerData.vida,
                0, /** absorbCounter - afegir referencia si cal */
                tempsFormatat
            );
        }
    }
}
