import sqlite3

# Conectar a la base de datos (asegúrate de que el archivo de la base de datos exista)
conn = sqlite3.connect('cartas.db')
c = conn.cursor()

# Datos de los enemigos a insertar
enemigos = [
    (1, 2, "¡Qué oportuno eres! No todos presencian un momento tan mágico... ¿Estás listo para ver un truco?"),
    (2, 3, "Sabía que vendrías... He guardado un mensaje solo para ti. ¿Te atreves a leer entre los sueños?"),
    (3, 4, "¡Qué suerte tienes! Suele costar encontrar al guía en tu camino... ¿Preparado para iluminarte?")
]

#Insertar los datos en la tabla
c.executemany("""
INSERT OR REPLACE INTO enemigos (id, carta_id, frase)
VALUES (?, ?, ?)
""", enemigos)


# Datos de las cartas
cartas = [
    (1, 'El Loco', 'Esta carta representa libertad y aventura.\nComienza tu cometido con mi poder.', 1, 7),
    (2, 'El Mago', 'Esta carta representa voluntad y creatividad.\nManifiesta mis poderes con propósito.', 2, 8),
    (3, 'La Sacerdotisa', 'Esta carta representa intuición y sabiduría.\nUsa mis poderes sabiamente.', 3, 9),
    (4, 'El Ermitaño', 'Esta carta representa reflexión y autoconocimiento.\nQue mis poderes te sirvan de guía.', 4, 10),
    (5, 'El Colgado', 'Esta carta representa pausa y entrega.\nSé consciente del poder que te otorgo.', 5, 11),
    (6, 'La Muerte', 'Esta carta representa transformación y renacimiento.\nCierra el círculo con mis poderes.', 6, 12)
]

# Insertar cartas (usamos INSERT OR REPLACE para evitar duplicados)
c.executemany("""
INSERT OR REPLACE INTO cartas (id, nombre, descripcion, set_mov_normales, set_mov_invertidos)
VALUES (?, ?, ?, ?, ?)
""", cartas)

# Datos de los enemigos
enemigos = [
    (1, 2, "¡Qué oportuno eres! No todos presencian un momento tan mágico... ¿Estás listo para ver un truco?"),
    (2, 3, "Sabía que vendrías... He guardado un mensaje solo para ti. ¿Te atreves a leer entre los sueños?"),
    (3, 4, "¡Qué suerte tienes! Suele costar encontrar al guía en tu camino... ¿Preparado para iluminarte?")
]

# Insertar enemigos
c.executemany("""
INSERT OR REPLACE INTO enemigos (id, carta_id, frase)
VALUES (?, ?, ?)
""", enemigos)

#datos de los movimientos
movimientos = [
    # Movimientos originales (IDs 1-18)
    (1, "Nuevos Comienzos", 6, 4),      # Muerte(6) vs Ermitaño(4)
    (2, "Espontaneidad", 5, 3),         # Colgado(5) vs Sacerdotisa(3)
    (3, "Fe en el Camino", 3, 5),       # Sacerdotisa(3) vs Colgado(5)
    (4, "Acción Consciente", 4, 6),     # Ermitaño(4) vs Muerte(6)
    (5, "Poder Personal", 3, 5),        # Sacerdotisa(3) vs Colgado(5)
    (6, "Manifestación", 1, 6),         # Loco(1) vs Muerte(6)
    (7, "Sabiduría Interior", 4, 2),    # Ermitaño(4) vs Mago(2)
    (8, "Ocultismo", 5, 6),             # Colgado(5) vs Muerte(6)
    (9, "Misterio", 6, 5),              # Muerte(6) vs Colgado(5)
    (10, "Búsqueda Interior", 3, 1),    # Sacerdotisa(3) vs Loco(1)
    (11, "Introspección", 2, 3),        # Mago(2) vs Sacerdotisa(3)
    (12, "Guía Espiritual", 6, 5),      # Muerte(6) vs Colgado(5)
    (13, "Pausa", 1, 2),                # Loco(1) vs Mago(2)
    (14, "Nueva Perspectiva", 2, 1),    # Mago(2) vs Loco(1)
    (15, "Iluminación espiritual", 4, 3), # Ermitaño(4) vs Sacerdotisa(3)
    (16, "Cierre de Ciclos", 1, 4),     # Loco(1) vs Ermitaño(4)
    (17, "Renacimiento", 4, 2),         # Ermitaño(4) vs Mago(2)
    (18, "Transformación", 2, 1),       # Mago(2) vs Loco(1)
    
    # Nuevos movimientos (IDs 19-36)
    (19, "Ingenuidad Peligrosa", 5, 6),       # Colgado(5) vs Muerte(6)
    (20, "Falta de Dirección", 6, 3),         # Muerte(6) vs Sacerdotisa(3)
    (21, "Decisión Impulsiva", 4, 5),         # Ermitaño(4) vs Colgado(5)
    (22, "Manipulación", 1, 5),               # Loco(1) vs Colgado(5)
    (23, "Desperdicio de Potencial", 6, 1),   # Muerte(6) vs Loco(1)
    (24, "Confusión de la Realidad", 5, 6),   # Colgado(5) vs Muerte(6)
    (25, "Silencio Interior", 2, 4),          # Mago(2) vs Ermitaño(4)
    (26, "Bloqueo Intuitivo", 6, 5),          # Muerte(6) vs Colgado(5)
    (27, "Proyección de Sombras", 4, 6),      # Ermitaño(4) vs Muerte(6)
    (28, "Aislamiento excesivo", 5, 2),       # Colgado(5) vs Mago(2)
    (29, "Negación de la Verdad", 3, 1),      # Sacerdotisa(3) vs Loco(1)
    (30, "Soledad", 2, 3),                    # Mago(2) vs Sacerdotisa(3)
    (31, "Evasión", 1, 4),                    # Loco(1) vs Ermitaño(4)
    (32, "Sacrificio inútil", 1, 2),          # Loco(1) vs Mago(2)
    (33, "Cambio Resistente", 3, 1),          # Sacerdotisa(3) vs Loco(1)
    (34, "Miedo al Cambio", 2, 4),            # Mago(2) vs Ermitaño(4)
    (35, "Estancamiento", 4, 3),              # Ermitaño(4) vs Sacerdotisa(3)
    (36, "Resistencia", 3, 2)                # Sacerdotisa(3) vs Mago(2)
]

# Insertar movimientos con IDs explícitos
c.executemany("""
INSERT OR REPLACE INTO movimientos (id, nombre, fuerte_contra, debil_contra)
VALUES (?, ?, ?, ?)
""", movimientos)

# Diccionario de movimientos (nombre -> id)
movimientos_dict = {
    # Derecho (1-18)
    "Nuevos Comienzos": 1,
    "Espontaneidad": 2,
    "Fe en el Camino": 3,
    "Acción Consciente": 4,
    "Poder Personal": 5,
    "Manifestación": 6,
    "Sabiduría Interior": 7,
    "Ocultismo": 8,
    "Misterio": 9,
    "Búsqueda Interior": 10,
    "Introspección": 11,
    "Guía Espiritual": 12,
    "Pausa": 13,
    "Nueva Perspectiva": 14,
    "Iluminación espiritual": 15,
    "Cierre de Ciclos": 16,
    "Renacimiento": 17,
    "Transformación": 18,
    # Inverso (19-36)
    "Ingenuidad Peligrosa": 19,
    "Falta de Dirección": 20,
    "Decisión Impulsiva": 21,
    "Manipulación": 22,
    "Desperdicio de Potencial": 23,
    "Confusión de la Realidad": 24,
    "Silencio Interior": 25,
    "Bloqueo Intuitivo": 26,
    "Proyección de Sombras": 27,
    "Aislamiento excesivo": 28,
    "Negación de la Verdad": 29,
    "Soledad": 30,
    "Evasión": 31,
    "Sacrificio inútil": 32,
    "Cambio Resistente": 33,
    "Miedo al Cambio": 34,
    "Estancamiento": 35,
    "Resistencia": 36
}

# Sets de movimientos (id, mov1, mov2, mov3)
sets_movimientos = [
    # Sets normales (1-6)
    (1, movimientos_dict["Nuevos Comienzos"], movimientos_dict["Espontaneidad"], movimientos_dict["Fe en el Camino"]),      # Loco
    (2, movimientos_dict["Acción Consciente"], movimientos_dict["Poder Personal"], movimientos_dict["Manifestación"]),      # Mago
    (3, movimientos_dict["Sabiduría Interior"], movimientos_dict["Ocultismo"], movimientos_dict["Misterio"]),               # Sacerdotisa
    (4, movimientos_dict["Búsqueda Interior"], movimientos_dict["Introspección"], movimientos_dict["Guía Espiritual"]),      # Ermitaño
    (5, movimientos_dict["Pausa"], movimientos_dict["Nueva Perspectiva"], movimientos_dict["Iluminación espiritual"]),       # Colgado
    (6, movimientos_dict["Cierre de Ciclos"], movimientos_dict["Renacimiento"], movimientos_dict["Transformación"]),         # Muerte
    
    # Sets invertidos (7-12)
    (7, movimientos_dict["Ingenuidad Peligrosa"], movimientos_dict["Falta de Dirección"], movimientos_dict["Decisión Impulsiva"]),    # Loco
    (8, movimientos_dict["Manipulación"], movimientos_dict["Desperdicio de Potencial"], movimientos_dict["Confusión de la Realidad"]), # Mago
    (9, movimientos_dict["Silencio Interior"], movimientos_dict["Bloqueo Intuitivo"], movimientos_dict["Proyección de Sombras"]),     # Sacerdotisa
    (10, movimientos_dict["Aislamiento excesivo"], movimientos_dict["Negación de la Verdad"], movimientos_dict["Soledad"]),           # Ermitaño
    (11, movimientos_dict["Evasión"], movimientos_dict["Sacrificio inútil"], movimientos_dict["Cambio Resistente"]),                  # Colgado
    (12, movimientos_dict["Miedo al Cambio"], movimientos_dict["Estancamiento"], movimientos_dict["Resistencia"])                     # Muerte
]

# Insertar sets de movimientos
c.executemany("""
INSERT OR REPLACE INTO set_movimientos (id, mov1, mov2, mov3)
VALUES (?, ?, ?, ?)
""", sets_movimientos)

# Verificar inserción
c.execute("SELECT COUNT(*) FROM set_movimientos")
count = c.fetchone()[0]

# Guardar cambios y cerrar conexión
conn.commit()
conn.close()

print("Base de datos creada y datos insertados correctamente!")
print(f"- Insertadas {len(cartas)} cartas")
print(f"- Insertados {len(enemigos)} enemigos")