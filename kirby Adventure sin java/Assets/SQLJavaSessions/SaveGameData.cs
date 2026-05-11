using System;
using System.IO;
using UnityEngine;

public class SaveGameData : MonoBehaviour
{
    /** ruta on es guardara el TXT */
    private string rutaTXT;

    void Start()
    {
        /** modifciar la ruta per a que sigui funcional tant en l'editor com en la build */
        #if UNITY_EDITOR
                /** a l'editor: Assets -> kirby Adventure -> ProjecteFinal */
                rutaTXT = Path.GetFullPath(Path.Combine(Application.dataPath, "../../ProjecteJava/kirby_stats.txt"));
        #else
            /** a la build: kirby Adventure_Data -> ProjecteFinal */
            rutaTXT = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjecteJava/kirby_stats.txt"));
        #endif
        Debug.Log("Ruta del TXT: " + rutaTXT);

        /** subscriure's al event de mort per guardar dades al morir (evita que no es guardin per la mort instantania) */
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDeadStart += GuardarDadesAlMorir;
        }
    }

    /** recull les dades actuals del joc i les guarda al fitxer TXT */
    public void GuardarDades()
    {
        try
        {
            /** agafem les dades directament del Kirby i del HUD */
            int vida = Kirby.instance != null ? Kirby.instance.HP : 0;
            int absorbCounter = Kirby.instance != null ? Kirby.instance.totalAbsorcions : 0;

            HUDKirby hud = FindObjectOfType<HUDKirby>();
            int score = hud != null ? hud.GetScore() : 0;

            /** Time.time son els segons transcorreguts des del inici de la partida*/
            int tempsSegons = Mathf.FloorToInt(Time.time); /** FloorToInt elimina els decimals*/
            string tempsFormatat = tempsSegons < 60 ?
                tempsSegons + " segons" :
                (tempsSegons / 60) + " minuts i " + (tempsSegons % 60) + " segons";

            /** escrivim les dades linia per linia al fitxer TXT */
            /** StreamWriter a false --> sobreescriure el fitxer existenten en lloc d'afegir dades al final (treballem per execucions no persistencia)*/
            StreamWriter escriptor = new StreamWriter(rutaTXT, false);
            escriptor.WriteLine("vida=" + vida);
            escriptor.WriteLine("absorbCounter=" + absorbCounter);
            escriptor.WriteLine("score=" + score);
            escriptor.WriteLine("tiempoDeJuego=" + tempsFormatat);
            escriptor.WriteLine("sessionTimestamp=" + DateTime.UtcNow.ToString("o")); /** "o" = format ISO 8601*/
            escriptor.Close();

            Debug.Log("Dades guardades al TXT correctament: vida=" + vida +
                " absorb=" + absorbCounter + " score=" + score);

        }
        catch (Exception e)
        {
            Debug.LogError("Error guardant el TXT: " + e.Message);
        }
    }

    /** llegeix les dades del fitxer TXT i les mostra per debug */
    public void LlegirDades()
    {
        try
        {
            if (!File.Exists(rutaTXT))
            {
                Debug.LogWarning("El fitxer TXT no existeix encara: " + rutaTXT);
                return;
            }

            StreamReader lector = new StreamReader(rutaTXT);
            string linia;
            Debug.Log("--- Llegint dades del TXT ---");
            while ((linia = lector.ReadLine()) != null)
            {
                Debug.Log(linia);
            }
            lector.Close();

        }
        catch (Exception e)
        {
            Debug.LogError("Error llegint el TXT: " + e.Message);
        }
    }

    /** es crida automaticament quan Kirby mor */
    private void GuardarDadesAlMorir()
    {
        Debug.Log("Kirby ha mort -> guardant dades al TXT...");
        GuardarDades();
    }

    /** quan el joc es tanca, guardem automaticament */
    private void OnApplicationQuit()
    {
        Debug.Log("Aplicacio tancada -> guardant dades al TXT...");
        GuardarDades();
    }

    private void OnDestroy()
    {
        /** dessubscriure's per evitar referencies penjants */
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDeadStart -= GuardarDadesAlMorir;
        }
    }
}