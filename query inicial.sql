USE [BDCrud]
GO

/****** Object:  Table [dbo].[Cliente]    Script Date: 4/09/2024 21:37:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[Clientes];
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TipoClientes]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[TipoClientes];
END
GO
CREATE TABLE [dbo].[TipoClientes](
	[id_tipocliente] [int]  NOT NULL,
	descripci�n [varchar](100) NOT NULL,	
	estado [bit]  NOT NULL,
 CONSTRAINT [PK_TipoClientes] PRIMARY KEY CLUSTERED 
(
	id_tipocliente ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO


INSERT INTO [dbo].[TipoClientes] (id_tipocliente, descripci�n, estado)
VALUES (1, 'Cliente Ordinario',1);

INSERT INTO [dbo].[TipoClientes] (id_tipocliente, descripci�n, estado)
VALUES (2, 'Cliente con Beneficio',1);


GO
CREATE TABLE [dbo].[Clientes](
	[id_cliente] [int]  NOT NULL,
	[nombres] [varchar](50) NOT NULL,
	[apellidos] [varchar](50) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[edad] [int]  NOT NULL,
	[estado] [bit]  NOT NULL,	
	id_tipocliente INT, -- Nueva columna que har� referencia a la tabla TipoCliente
    CONSTRAINT FK_Clientes_TipoCliente FOREIGN KEY (id_tipocliente)
    REFERENCES [TipoClientes](id_tipocliente), -- Definici�n de la clave for�nea



 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[id_cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

INSERT INTO [dbo].[Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (1, 'Juan', 'P�rez', 'juan.perez@example.com', 28,1,1);

INSERT INTO [dbo].[Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (2, 'Mar�a', 'Gonz�lez', 'maria.gonzalez@example.com', 54,1,2);

INSERT INTO [dbo].[Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (3, 'Carlos', 'Rodr�guez', 'carlos.rodriguez@example.com', 55,1,2);

INSERT INTO [dbo].[Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (4, 'Ana', 'L�pez', 'ana.lopez@example.com', 29,1,1);

INSERT INTO [dbo].[Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (5, 'Pedro', 'Mart�nez', 'pedro.martinez@example.com', 38,1,1);


GO
ALTER TABLE [dbo].[Clientes]
ADD [fecha_nacimiento] DATE;
go

UPDATE [dbo].[Clientes]
SET [fecha_nacimiento] = '1900-01-01';

GO


select * from [Clientes];

select * from TipoClientes;