ALTER procedure [sp_ListaClientesxId]
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





ALTER procedure [sp_EliminaClientexId]
@id_cliente	int
as
begin

DECLARE @Exito AS VARCHAR(250)
DECLARE @Msg AS VARCHAR(250)
DECLARE @Ok AS BIT

update c set c.estado=0
from Clientes C  where C.id_cliente=@id_cliente

end

-------------------




ALTER procedure [sp_CreaCliente]

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


/*
exec [sp_CreaCliente] 'ANTHONY','VILLANUEVA', 'AMVA@GMAIL.COM','1944-10-08';
*/



ALTER procedure [sp_UpdateCliente]
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




/*
update CUENTAS_CORRIENTES_DETAIL_PAGOS 
	set Eliminado=1 ,
		UsuarioDig	=@UserId, 
		WorkStationDig	=@WorkStation, 
		FechaDig	=getdate()
	where Campana=@Campana and Predio=@Predio and Concepto=@Concepto and
		Numero_Pago=@Numero_Pago and NumeroDoc=@NumeroDoc and Eliminado=0*/
