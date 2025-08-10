import sqlite3

# Nombre del archivo de la base de datos
DB_NAME = "cartas.db"

def create_db():
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()

    # Activar llaves foráneas
    c.execute("PRAGMA foreign_keys = ON;")

    # Tabla de movimientos
    c.execute("""
    CREATE TABLE IF NOT EXISTS movimientos (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        nombre TEXT NOT NULL,
        fuerte_contra INTEGER,
        debil_contra INTEGER,
        FOREIGN KEY (fuerte_contra) REFERENCES cartas(id),
        FOREIGN KEY (debil_contra) REFERENCES cartas(id)
    );
    """)

    # Tabla de set de movimientos
    c.execute("""
    CREATE TABLE IF NOT EXISTS set_movimientos (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        mov1 INTEGER NOT NULL,
        mov2 INTEGER NOT NULL,
        mov3 INTEGER NOT NULL,
        FOREIGN KEY (mov1) REFERENCES movimientos(id),
        FOREIGN KEY (mov2) REFERENCES movimientos(id),
        FOREIGN KEY (mov3) REFERENCES movimientos(id)
    );
    """)

    # Tabla de cartas
    c.execute("""
    CREATE TABLE IF NOT EXISTS cartas (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        nombre TEXT NOT NULL,
        descripcion TEXT NOT NULL,
        set_mov_normales INTEGER NOT NULL,
        set_mov_invertidos INTEGER NOT NULL,
        FOREIGN KEY (set_mov_normales) REFERENCES set_movimientos(id),
        FOREIGN KEY (set_mov_invertidos) REFERENCES set_movimientos(id)
    );
    """)



    # Tabla de enemigos
    c.execute("""
    CREATE TABLE IF NOT EXISTS enemigos (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        carta_id INTEGER NOT NULL,
        frase TEXT NOT NULL,
        FOREIGN KEY (carta_id) REFERENCES cartas(id)
    );
    """)

    # Tabla de progreso
    c.execute("""
    CREATE TABLE IF NOT EXISTS progreso (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        enemigo_id INTEGER NOT NULL,
        fecha_derrota TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (enemigo_id) REFERENCES enemigos(id)
    );
    """)
    conn.commit()
    conn.close()
    print(f"Base de datos '{DB_NAME}' creada correctamente.")

if __name__ == "__main__":
    create_db()
