INSERT INTO "MetodosPago" ("Nombre", "Activo") VALUES ('Efectivo', true);

INSERT INTO "Roles" ("Nombre", "Descripcion", "Activo") VALUES ('Administrador', 'Acceso Total', true);
INSERT INTO "Roles" ("Nombre", "Descripcion", "Activo") VALUES ('Vendedor', 'Venta a Clientes', true);

INSERT INTO "Usuarios" ("Username", "PasswordHash", "Nombre", "Apellido", "Cedula", "Email", "RoleId", "Activo", "Bloqueado", "IntentosFallidos", "FechaCreacion", "RefreshToken", "RefreshTokenExpiryTime")
VALUES ('admin', '$2a$10$8Wi4qkRoWLqIEMF5yG9l8eCD6V9Db4ZPJGch/raTTVH95ModZSDs.', 'Administrador', 'Sistema', '1802288996', 'admin@empresa.com', 1, true, false, 0, NOW(), NULL, NULL);