USE [BDCrud]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombres] [nvarchar](max) NOT NULL,
	[apellidos] [nvarchar](max) NOT NULL,
	[fechaNac] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[id_cliente] [int] NOT NULL,
	[nombres] [varchar](50) NOT NULL,
	[apellidos] [varchar](50) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[edad] [int] NOT NULL,
	[estado] [bit] NOT NULL,
	[id_tipocliente] [int] NULL,
	[fecha_nacimiento] [date] NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[id_cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoClientes]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoClientes](
	[id_tipocliente] [int] NOT NULL,
	[descripción] [varchar](100) NOT NULL,
	[estado] [bit] NOT NULL,
 CONSTRAINT [PK_TipoClientes] PRIMARY KEY CLUSTERED 
(
	[id_tipocliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [FK_Clientes_TipoCliente] FOREIGN KEY([id_tipocliente])
REFERENCES [dbo].[TipoClientes] ([id_tipocliente])
GO
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Clientes_TipoCliente]
GO
/****** Object:  StoredProcedure [dbo].[sp_CreaCliente]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[sp_CreaCliente]

@nombres VARCHAR(50) ,
@apellidos VARCHAR(50),
@email VARCHAR(100) ,
@fecha_nacimiento DATE


as
begin

DECLARE @Msg AS VARCHAR(250)
DECLARE @Ok AS BIT


DECLARE @id  INTEGER
DECLARE @id_tipoCliente  INTEGER
DECLARE @edad  INTEGER
 SET @Ok=0;


IF EXISTS (SELECT 1 FROM Clientes C where C.nombres=LTRIM(RTRIM(@nombres)) and C.apellidos=LTRIM(RTRIM(@apellidos)))
BEGIN
   SET @Msg='Ya existe un Cliente con los mismos datos';  
   GOTO Terminar           
 
END


BEGIN TRAN
BEGIN TRY 

SELECT  @id=max(ISNULL(c.id_cliente,0))+1 FROM Clientes c 

SET @Edad = DATEDIFF(YEAR, @fecha_nacimiento, GETDATE()) 
        - CASE 
            WHEN DATEADD(YEAR, DATEDIFF(YEAR, @fecha_nacimiento, GETDATE()), @fecha_nacimiento) > GETDATE() 
            THEN 1 
            ELSE 0 
          END;

INSERT INTO [Clientes] (id_cliente, nombres, apellidos, email, edad,estado,id_tipocliente)
VALUES (@id, @nombres, @apellidos, @email, @edad,1,IIF(@Edad>50,2,1));
 SET @Ok=1;
 SET @Msg='1=INSERTADO CORRECTAMENTE';



END TRY
	BEGIN CATCH
			SET @Msg = RTRIM(LTRIM(STR(ABS(ERROR_NUMBER()) * -1))) + '=' + ERROR_MESSAGE()+
							'ErrorLine:'+ CAST(ERROR_LINE() AS VARCHAR(10))
			IF @@TRANCOUNT > 0 ROLLBACK;
				GOTO Terminar            
	END CATCH
	IF @@TRANCOUNT > 0 
	BEGIN            
		COMMIT;            
	END

	Terminar:
	SELECT @Msg Msg,@Ok Ok
end
GO
/****** Object:  StoredProcedure [dbo].[sp_EliminaClientexId]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_EliminaClientexId]
@id_cliente	int
as
begin

DECLARE @Exito AS VARCHAR(250)
DECLARE @Msg AS VARCHAR(250)
DECLARE @Ok AS BIT

update c set c.estado=0
from Clientes C  where C.id_cliente=@id_cliente

end
GO
/****** Object:  StoredProcedure [dbo].[sp_ListaClientesxId]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_ListaClientesxId]
@id_cliente	int,
@id_tipocliente int

as
begin

select c.id_cliente,C.id_tipocliente,C.nombres, C.apellidos,c.email,c.edad,c.fecha_nacimiento,c.estado
from Clientes C 
where (C.id_cliente=@id_cliente or @id_cliente=0) 
	and (c.id_tipocliente=@id_tipocliente or @id_tipocliente=0)
	and c.estado=1;


end
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateCliente]    Script Date: 5/09/2024 03:18:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_UpdateCliente]
@id_cliente	int,
@nombres VARCHAR(50) ,
@apellidos VARCHAR(50),
@email VARCHAR(100) ,
@fecha_nacimiento DATE


as
begin

DECLARE @Msg AS VARCHAR(250)
DECLARE @Ok AS BIT
DECLARE @edad  INTEGER
 SET @Ok=0;

BEGIN TRAN
BEGIN TRY 

SET @Edad = DATEDIFF(YEAR, @fecha_nacimiento, GETDATE()) 
        - CASE 
            WHEN DATEADD(YEAR, DATEDIFF(YEAR, @fecha_nacimiento, GETDATE()), @fecha_nacimiento) > GETDATE() 
            THEN 1 
            ELSE 0 
          END;

UPDATE C 
	SET C.nombres=@nombres, C.apellidos=@apellidos, C.email=@email,
	C.edad=@Edad, c.fecha_nacimiento=@fecha_nacimiento, c.id_tipocliente=IIF(@Edad>50,2,1)
FROM Clientes C 
WHERE C.id_cliente=@id_cliente
 SET @Ok=1;
 SET @Msg='1=ACTUALIZADO CORRECTAMENTE';


END TRY
	BEGIN CATCH
			SET @Msg = RTRIM(LTRIM(STR(ABS(ERROR_NUMBER()) * -1))) + '=' + ERROR_MESSAGE()+
							'ErrorLine:'+ CAST(ERROR_LINE() AS VARCHAR(10))
			IF @@TRANCOUNT > 0 ROLLBACK;
				GOTO Terminar            
	END CATCH
	IF @@TRANCOUNT > 0 
	BEGIN            
		COMMIT;            
	END

	Terminar:
	SELECT @Msg Msg,@Ok Ok

end
GO
