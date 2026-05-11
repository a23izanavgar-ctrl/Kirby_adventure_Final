import java.sql.*;
import java.io.*; /** per comprovar i rescatar erros entrada/sortida */
import java.util.Scanner;

/**
 * Programa principal de gestio d'estadistiques de Kirby en SQL.
 *
 * Flux del programa:
 * 1 --> Unity genera un fitxer TXT amb les estadistiques de la partida
 * 2 --> Aquest programa llegeix el TXT
 * 3 --> Les dades es poden inserir a MySQL
 * 4 --> Tambe es poden consultar o modificar sessions desades
 */

/**programa principal --> llegeix el .txt que genera Unity i el gestiona a MySQL*/
public class KirbyStats {

    /** credencials de connexio a la base de dades local (utilitzem XAMPP) */
    static final String URL = "jdbc:mysql://localhost:3306/KirbysAdventure?useSSL=false&serverTimezone=UTC";
    static final String USUARI = "root";
    static final String CONTRASENYA = "";

    /** ruta relativa del TXT generat per Unity (es guardara a la mateixa carpeta) */
    static final String RUTA_TXT = "kirby_stats.txt";

    public static void main(String[] args) {
        Scanner entrada = new Scanner(System.in);
        int opcio;

        do {
            System.out.printf("\n- - - - - - M E N U - - - - - -\n");
            System.out.printf("1. Llegir TXT\n");
            System.out.printf("2. Inserir dades a la BDD\n");
            System.out.printf("3. Mostrar totes les sessions\n");
            System.out.printf("4. Actualitzar vida d'una sessio\n");
            System.out.printf("0. Sortir\n");
            System.out.printf("Opcio --> ");

            try {
                opcio = entrada.nextInt();
            } catch (Exception e) { /** capturar possibles errors d'entrada */
                System.out.printf("Introdueix un valor valid!\n");
                entrada.nextLine();
                opcio = -1; /** continuar... */
            }
            entrada.nextLine();

            switch (opcio) {
                case 1:
                    System.out.printf("Opcio 1 -> llegint TXT...\n");
                    llegirTXT();
                    break;
                case 2:
                    System.out.printf("Opcio 2 -> inserint dades a la BDD...\n");
                    inserirDesDelTXT();
                    break;
                case 3:
                    System.out.printf("Opcio 3 -> mostrant totes les sessions...\n");
                    mostrarSessions();
                    break;
                case 4:
                    System.out.printf("Opcio 4 -> actualitzant vida d'una sessio...\n");
                    actualitzarVida(entrada);
                    break;
                case 0:
                    System.out.printf("Opcio 0 -> sortint del programa...\n");
                    break;
                default:
                    if (opcio != -1) {
                        System.out.printf("Aquesta opcio no pot ser!\n");
                    }
            }
        } while (opcio != 0);

        System.out.printf("Sortint...\n");
        entrada.close();
    }

    /**opcio 1 --> llegeix el fitxer TXT i mostra el seu contingut per pantalla*/
    private static void llegirTXT() {
        try { /** intentar llegir/mostrar */
            File fitxer = new File(RUTA_TXT); /** obtindre la ruta */
            if (!fitxer.exists()) {
                System.out.printf("ERROR --> el fitxer TXT no existeix: %s\n", RUTA_TXT);
                return;
            }

            Scanner lector = new Scanner(fitxer); /**nou escanejador que llegira el fitxer */
            System.out.printf("\nContigut del TXT llegit:\n");
            while (lector.hasNextLine()) {
                System.out.printf("%s\n", lector.nextLine());
            }
            lector.close();
            System.out.printf("\n");

        } catch (FileNotFoundException e) { /** sino, capturar error */
            System.out.printf("Error llegint el fitxer: %s\n", e.getMessage());
        }
    }

    /**extreu les dades del TXT i les retorna com a array per inserir a la BDD*/
    private static String[] extreureDades() {
        try {
            File fitxer = new File(RUTA_TXT);
            if (!fitxer.exists()) {
                System.out.printf("ERROR --> el fitxer TXT no existeix: %s\n", RUTA_TXT);
                return null;
            }

            /** valors per defecte (si alguna dada no existeix al TXT) */
            String vida = "0";
            String absorbCounter = "0";
            String score = "0";
            String tiempoDeJuego = "";
            String sessionTimestamp = "";

            Scanner lector = new Scanner(fitxer);
            while (lector.hasNextLine()) {
                String linia = lector.nextLine();

                /** extraiem el valor de cada linia (format: clau=valor) */
                if (linia.startsWith("vida=")) {
                    /** substring(5) elimina "vida=" i es queda nomes amb el valor */
                    vida = linia.substring(5); 
                } else if (linia.startsWith("absorbCounter=")) {
                    /** igual per a tots els altres... */
                    absorbCounter = linia.substring(14);
                } else if (linia.startsWith("score=")) {
                    score = linia.substring(6);
                } else if (linia.startsWith("tiempoDeJuego=")) {
                    tiempoDeJuego = linia.substring(14);
                } else if (linia.startsWith("sessionTimestamp=")) {
                    sessionTimestamp = linia.substring(17);
                }
            }
            lector.close();

            return new String[]{vida, absorbCounter, score, tiempoDeJuego, sessionTimestamp};

        } catch (FileNotFoundException e) {
            System.out.printf("Error llegint el fitxer: %s\n", e.getMessage());
            return null;
        }
    }

    /**opcio 2 --> insereix les dades del TXT a la BDD*/
    private static void inserirDesDelTXT() {
        String[] dades = extreureDades();
        if (dades == null) {
            return;
        }

        int vida = Integer.parseInt(dades[0].isEmpty() ? "0" : dades[0]);
        int absorbCounter = Integer.parseInt(dades[1].isEmpty() ? "0" : dades[1]);
        int score = Integer.parseInt(dades[2].isEmpty() ? "0" : dades[2]);
        String tiempoDeJuego = dades[3];
        String sessionTimestamp = dades[4];

        /** try-with-resources -> la connexio es tanca automaticament al sortir del bloc try*/
        try (Connection conn = DriverManager.getConnection(URL, USUARI, CONTRASENYA)) {
            System.out.printf("Connexio establerta correctament!\n");

            /** PreparedStatement permet inserir valors de forma segura i evitar injeccions SQL */
            String sql = "INSERT INTO sessions (vida, absorbCounter, score, tiempoDeJuego, sessionTimestamp) VALUES (?, ?, ?, ?, ?)";
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, vida);
            ps.setInt(2, absorbCounter);
            ps.setInt(3, score);
            ps.setString(4, tiempoDeJuego);
            ps.setString(5, sessionTimestamp);

            int files = ps.executeUpdate();
            System.out.printf("Insercio correcta! Files afectades: %d\n", files);
            System.out.printf("Dades inserides --> vida: %d | absorb: %d | score: %d | temps: %s | timestamp: %s\n",
                vida, absorbCounter, score, tiempoDeJuego, sessionTimestamp);

            ps.close();
        } catch (SQLException e) {
            System.out.printf("Error SQL: %s\n", e.getMessage());
        }
    }

    /**opcio 3 --> mostra totes les sessions de la base de dades*/
    private static void mostrarSessions() {
        try (Connection conn = DriverManager.getConnection(URL, USUARI, CONTRASENYA)) {
            System.out.printf("Connexio establerta correctament!\n");

            Statement stmt = conn.createStatement();
            ResultSet rs = stmt.executeQuery("SELECT * FROM sessions");

            /** mostrar dades alineades per columnes */
            System.out.printf("%-5s %-6s %-10s %-8s %-20s %-30s\n",
                "ID", "VIDA", "ABSORB", "SCORE", "TEMPS", "TIMESTAMP");
            System.out.printf("%-5s %-6s %-10s %-8s %-20s %-30s\n",
                "---", "------", "----------", "--------", "--------------------", "------------------------------");

            int total = 0;
            while (rs.next()) {
                total++;
                System.out.printf("%-5d %-6d %-10d %-8d %-20s %-30s\n",
                    rs.getInt("id"),
                    rs.getInt("vida"),
                    rs.getInt("absorbCounter"),
                    rs.getInt("score"),
                    rs.getString("tiempoDeJuego"),
                    rs.getString("sessionTimestamp"));
            }
            System.out.printf("\nTotal de sessions: %d\n", total);

            rs.close();
            stmt.close();
        } catch (SQLException e) {
            System.out.printf("Error SQL: %s\n", e.getMessage());
        }
    }

    /**opcio 4 --> actualitza la vida d'una sessio per id*/
    private static void actualitzarVida(Scanner entrada) {
        try {
            System.out.printf("ID de la sessio a actualitzar --> ");
            int id = entrada.nextInt();
            entrada.nextLine();

            System.out.printf("Nova vida --> ");
            int novaVida = entrada.nextInt();
            entrada.nextLine();

            try (Connection conn = DriverManager.getConnection(URL, USUARI, CONTRASENYA)) {
                System.out.printf("Connexio establerta correctament!\n");

                String sql = "UPDATE sessions SET vida = ? WHERE id = ?";
                PreparedStatement ps = conn.prepareStatement(sql);
                ps.setInt(1, novaVida);
                ps.setInt(2, id);

                int files = ps.executeUpdate();
                if (files > 0) {
                    System.out.printf("Actualitzacio correcta! Sessio %d -> vida = %d\n", id, novaVida);
                } else {
                    System.out.printf("No s'ha trobat cap sessio amb id = %d\n", id);
                }
                ps.close();
            } catch (SQLException e) {
                System.out.printf("Error SQL: %s\n", e.getMessage());
            }
        } catch (Exception e) {
            System.out.printf("Valor no valid: %s\n", e.getMessage());
            entrada.nextLine();
        }
    }
}