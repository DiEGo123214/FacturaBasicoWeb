psql -U postgres

select * from Productos;

select * from Clientes where Identificacion = '1118918026';
-------------------
migrar a postgres
1. Actualizar CLI de Entity Framework
dotnet tool update --global dotnet-ef

2. Verificar driver ODBC de SQL Server
Get-OdbcDriver | Where-Object { $_.Name -like "*SQL Server*" } | Select-Object Name
Anota si dice 17 o 18, lo necesitas en el paso 9.
--
PASO 1 —  Cambiar paquetes NuGet en POS.Infrastructure
cd backend/src/POS.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.0-preview.5
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0-preview.5.25277.114

PASO 2 — Actualizar paquete en POS.API
cd ../POS.API
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0-preview.5.25277.114

Archivo: backend/src/POS.Infrastructure/POS.Infrastructure.csproj
<!-- QUITAR esta línea -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0-preview.3.25171.6" />
y poner 
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0-preview.4" />

PASO 3 — Editar POS.Infrastructure.csproj
Archivo: backend/src/POS.Infrastructure/POS.Infrastructure.csproj
quitar
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="..." />
dejar solo
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0-preview.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0-preview.5.25277.114" />

PASO 4 — Cambiar UseSqlServer → UseNpgsql

Archivo: backend/src/POS.Infrastructure/DependencyInjection.cs
// ANTES
options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
// DESPUÉS
options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))

PASO 5 — Cambiar cadena de conexión
Archivo: backend/src/POS.API/appsettings.json
"DefaultConnection": "Host=localhost;Port=5432;Database=pos_pg;Username=postgres;Password=DIEgo;"

PASO 6 — Crear base de datos en PostgreSQL
psql -U postgres
CREATE DATABASE pos_pg;

PASO 7 — Borrar migraciones viejas
cd backend
Remove-Item -Recurse -Force src\POS.Infrastructure\Migrations --> no importa si no vale


PASO 8 — Crear y aplicar migración nueva
cd src/POS.API
dotnet ef migrations add InitialCreate --project ..\POS.Infrastructure
dotnet ef database update  --> no importa si sale un erro

PASO 9 — Instalar dependencias Python
cd backend/migracion
py -m pip install -r requirements.txt

PASO 10 — Correr el seed y la migración de datos
# Seed (datos base: roles, usuario admin, método de pago)
psql -U postgres -d pos_pg -f seed.sql

# Migración de clientes y productos con ID impar
py migrar.py


DROP DATABASE nombre_bd;
select * from "Clientes" where "Activo" = false;
------------------------------------------

regresar a sql server
PASO 1 — Restaurar paquete NuGet
cd backend/src/POS.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0-preview.3.25171.6

En POS.Infrastructure.csproj

<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0-preview.4" /> ES DE POSTGRES Y PONER ESTO DEBE HABER 2 

<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0-preview.3.25171.6" />

PASO 2 — Cambiar UseNpgsql a UseSqlServer

Archivo: backend/src/POS.Infrastructure/DependencyInjection.cs

options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); //SQL SERVER

PASO 2.5 — Borrar migraciones de Postgres
cd backend
Remove-Item -Recurse -Force src\POS.Infrastructure\Migrations


PASO 3 — Restaurar cadena de conexión

Archivo: backend/src/POS.API/appsettings.json
"DefaultConnection": "Server=DESKTOP-B1BSUC4;Database=PuntoDeVentaDB;Trusted_Connection=true;TrustServerCertificate=true;Pooling=true;Max Pool Size=100;MultipleActiveResultSets=true;"
--------------
INSERT INTO MetodosPago (Nombre, Activo)
VALUES ('Efectivo',1);

INSERT INTO Roles (Nombre,Descripcion,Activo)
VALUES ('Administrador','Acceso Total',1);

INSERT INTO Roles (Nombre,Descripcion,Activo)
VALUES ('Vendedor','Venta a Clientes',1);

INSERT INTO Usuarios (Username,PasswordHash,Nombre,Apellido,Cedula,Email,RoleId,Activo,Bloqueado,IntentosFallidos,FechaCreacion,RefreshToken,RefreshTokenExpiryTime)
VALUES ('admin','$2a$10$8Wi4qkRoWLqIEMF5yG9l8eCD6V9Db4ZPJGch/raTTVH95ModZSDs.','Administrador','Sistema','1802288996','admin@empresa.com',1,1,0,0,GETDATE(),NULL,NULL);

------------
la aplicacion web ya pemite qye un vendedor 

al seleccionar esta opcion (Vender nuevamente ) la aplicacion devera intertar cargar(copiar) en la venta actual los productos y cantidades
registrados en la venta historica seleeccionada.
antes de realizar esta operacion, la aplicacion debera verificar que todos los 
amigo ahora quiero un boton para poder duplicar la factura, osea una factura que esta echa y eso quiero copiar a la venta donde seleccionamos
el cliente y productos pero de una factura que esta hecha 
debe cumplir si los productos cumplen con condicion de stock, de venta deber copiarse a la pantalla con cliente y producto
si al menos un producto no cumplen con la condicion de stock, no debera copiarse ningun producto a la venta y que diga cuales y porque
no se puede hacer un mensaje 
en caso de que  no pueda completar (es decir no pueda copiar  mande toda la infromacion el producto y el stock toda la informacion )
la duplicacion no se debe guardar en la base de datos
------------
perfecto ahora vamos con esta parte de 
si el numero de items de la venta historica es par y la cantidad total de la venta historica es par entonces la venta se copia caso contratio
no se copia la venta- un mensaje no se puede copiar
osea veras entramos a una factura y vemos cuantos productos estan seleccionadas si es un numero par y que el total sea par se puede copiar 
caso contrario si cumple eso mostrar un mensaje no se puede copiar ya hice algunos cambios y de igual forma aumneta otro boton para hacer eso
