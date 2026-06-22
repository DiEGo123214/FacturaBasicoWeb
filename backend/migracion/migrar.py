import pyodbc
import psycopg2
from decimal import Decimal

# ─── CONFIGURACIÓN ───────────────────────────────────────────
SQL_SERVER = {
    "driver": "ODBC Driver 17 for SQL Server",
    "server": "DESKTOP-B1BSUC4",
    "database": "PuntoDeVentaDB",
}

POSTGRES = {
    "host": "localhost",
    "port": 5432,
    "database": "pos_pg",
    "user": "postgres",
    "password": "DIEgo"
}

def conectar_sqlserver():
    conn_str = (
        f"DRIVER={{ODBC Driver 17 for SQL Server}};"
        f"SERVER=DESKTOP-B1BSUC4;"
        f"DATABASE=PuntoDeVentaDB;"
        f"Trusted_Connection=yes;"
        f"TrustServerCertificate=yes;"
    )
    return pyodbc.connect(conn_str)

def conectar_postgres():
    return psycopg2.connect(
        host=POSTGRES["host"],
        port=POSTGRES["port"],
        dbname=POSTGRES["database"],
        user=POSTGRES["user"],
        password=POSTGRES["password"]
    )

def migrar():
    print("Conectando a SQL Server...")
    sql = conectar_sqlserver()
    sql_cursor = sql.cursor()

    print("Conectando a PostgreSQL...")
    pg = conectar_postgres()
    pg_cursor = pg.cursor()

    try:
        # ── 1. CLIENTES con ID impar ──────────────────────────
        print("\n→ Migrando Clientes...")
        sql_cursor.execute("SELECT Id, Identificacion, Nombre, Apellido, Direccion, Telefono, Email, FechaCreacion, Activo FROM Clientes WHERE Id % 2 = 1")
        clientes = sql_cursor.fetchall()
        for row in clientes:
            pg_cursor.execute("""
                INSERT INTO "Clientes" ("Id", "Identificacion", "Nombre", "Apellido", "Direccion", "Telefono", "Email", "FechaCreacion", "Activo")
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
                ON CONFLICT ("Id") DO NOTHING
            """, (row.Id, row.Identificacion, row.Nombre, row.Apellido, row.Direccion, row.Telefono, row.Email, row.FechaCreacion, bool(row.Activo)))
        print(f"  ✅ {len(clientes)} clientes migrados")

        # ── 2. PRODUCTOS con ID impar ─────────────────────────
        print("\n→ Migrando Productos...")
        sql_cursor.execute("SELECT Id, Codigo, Nombre, Descripcion, Precio, Stock, FechaCreacion, Activo FROM Productos WHERE Id % 2 = 1")
        productos = sql_cursor.fetchall()
        for row in productos:
            pg_cursor.execute("""
                INSERT INTO "Productos" ("Id", "Codigo", "Nombre", "Descripcion", "Precio", "Stock", "FechaCreacion", "Activo")
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
                ON CONFLICT ("Id") DO NOTHING
             """, (row.Id, row.Codigo, row.Nombre, row.Descripcion, Decimal(str(row.Precio)), row.Stock, row.FechaCreacion, row.Activo))
        print(f"  ✅ {len(productos)} productos migrados")

        pg.commit()
        print("\n✅ Migración completada exitosamente!")

    except Exception as e:
        pg.rollback()
        print(f"\n❌ Error durante la migración: {e}")
        raise
    finally:
        sql_cursor.close()
        sql.close()
        pg_cursor.close()
        pg.close()

if __name__ == "__main__":
    migrar()