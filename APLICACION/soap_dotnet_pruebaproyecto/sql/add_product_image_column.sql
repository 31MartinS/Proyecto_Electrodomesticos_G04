USE ComercializadoraDB;
GO

-- Agrega la columna image_filename para almacenar el nombre del archivo de imagen
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'PRODUCTO' AND COLUMN_NAME = 'image_filename'
)
BEGIN
    ALTER TABLE PRODUCTO ADD image_filename VARCHAR(255) NULL;
    PRINT 'Columna image_filename añadida a PRODUCTO.';
END
ELSE
BEGIN
    PRINT 'La columna image_filename ya existe.';
END
