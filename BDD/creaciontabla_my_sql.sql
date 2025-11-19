/* =========================================================
   BASE DE DATOS BANCO BANQUITO (CORE + CRÉDITO)
   ========================================================= */
CREATE DATABASE BanQuitoDB;
GO

USE BanQuitoDB;
GO

/* ---------- TABLA CLIENTE ---------- */
CREATE TABLE CLIENTE (
    id_cliente        INT IDENTITY(1,1) PRIMARY KEY,
    cedula            VARCHAR(20) NOT NULL UNIQUE,
    nombres           VARCHAR(100) NOT NULL,
    apellidos         VARCHAR(100) NOT NULL,
    fecha_nacimiento  DATE NOT NULL,
    estado_civil      VARCHAR(20) NOT NULL,
    CONSTRAINT CK_CLIENTE_EstadoCivil
        CHECK (estado_civil IN ('SOLTERO', 'CASADO', 'DIVORCIADO', 'VIUDO', 'UNION_LIBRE'))
);

/* ---------- TABLA CUENTA ---------- */
CREATE TABLE CUENTA (
    id_cuenta       INT IDENTITY(1,1) PRIMARY KEY,
    numero_cuenta   VARCHAR(20) NOT NULL UNIQUE,
    id_cliente      INT NOT NULL,
    tipo_cuenta     VARCHAR(20) NOT NULL,
    saldo           DECIMAL(18,2) NOT NULL DEFAULT 0,
    fecha_apertura  DATE NOT NULL,
    CONSTRAINT FK_CUENTA_Cliente
        FOREIGN KEY (id_cliente) REFERENCES CLIENTE(id_cliente)
);

/* ---------- TABLA MOVIMIENTO ---------- */
CREATE TABLE MOVIMIENTO (
    id_movimiento     INT IDENTITY(1,1) PRIMARY KEY,
    id_cuenta         INT NOT NULL,
    fecha_movimiento  DATETIME2 NOT NULL,
    tipo_movimiento   VARCHAR(20) NOT NULL,
    monto             DECIMAL(18,2) NOT NULL,
    descripcion       VARCHAR(255) NULL,
    CONSTRAINT FK_MOVIMIENTO_Cuenta
        FOREIGN KEY (id_cuenta) REFERENCES CUENTA(id_cuenta),
    CONSTRAINT CK_MOV_TipoMovimiento
        CHECK (tipo_movimiento IN ('DEPOSITO', 'RETIRO'))
);

/* ---------- TABLA CREDITO ---------- */
CREATE TABLE CREDITO (
    id_credito     INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente     INT NOT NULL,
    id_cuenta      INT NOT NULL,
    monto_credito  DECIMAL(18,2) NOT NULL,
    plazo_meses    INT NOT NULL,
    tasa_anual     DECIMAL(5,2) NOT NULL DEFAULT 16.00,  -- 16% anual
    fecha_inicio   DATE NOT NULL,
    estado         VARCHAR(20) NOT NULL,
    CONSTRAINT FK_CREDITO_Cliente
        FOREIGN KEY (id_cliente) REFERENCES CLIENTE(id_cliente),
    CONSTRAINT FK_CREDITO_Cuenta
        FOREIGN KEY (id_cuenta) REFERENCES CUENTA(id_cuenta),
    CONSTRAINT CK_CREDITO_Estado
        CHECK (estado IN ('ACTIVO', 'CANCELADO', 'RECHAZADO'))
);

/* ---------- TABLA CUOTA_CREDITO (AMORTIZACIÓN) ---------- */
CREATE TABLE CUOTA_CREDITO (
    id_cuota          INT IDENTITY(1,1) PRIMARY KEY,
    id_credito        INT NOT NULL,
    numero_cuota      INT NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    valor_cuota       DECIMAL(18,2) NOT NULL,
    interes_pagado    DECIMAL(18,2) NOT NULL,
    capital_pagado    DECIMAL(18,2) NOT NULL,
    saldo_restante    DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_CUOTA_CREDITO_Credito
        FOREIGN KEY (id_credito) REFERENCES CREDITO(id_credito)
);
GO

/* =========================================================
   BASE DE DATOS COMERCIALIZADORA DE ELECTRODOMÉSTICOS
   ========================================================= */
CREATE DATABASE ComercializadoraDB;
GO

USE ComercializadoraDB;
GO

/* ---------- TABLA CLIENTE_COMERCIALIZADORA ---------- */
CREATE TABLE CLIENTE_COMERCIALIZADORA (
    id_cliente   INT IDENTITY(1,1) PRIMARY KEY,
    cedula       VARCHAR(20) NOT NULL UNIQUE,
    nombres      VARCHAR(100) NOT NULL,
    apellidos    VARCHAR(100) NOT NULL,
    direccion    VARCHAR(200) NULL,
    telefono     VARCHAR(20) NULL,
    email        VARCHAR(100) NULL
);

/* ---------- TABLA PRODUCTO (ELECTRODOMÉSTICOS) ---------- */
CREATE TABLE PRODUCTO (
    id_producto   INT IDENTITY(1,1) PRIMARY KEY,
    codigo        VARCHAR(20) NOT NULL UNIQUE,
    nombre        VARCHAR(100) NOT NULL,
    descripcion   VARCHAR(255) NULL,
    precio_venta  DECIMAL(18,2) NOT NULL
);

/* ---------- TABLA FACTURA ---------- */
CREATE TABLE FACTURA (
    id_factura        INT IDENTITY(1,1) PRIMARY KEY,
    numero_factura    VARCHAR(30) NOT NULL UNIQUE,
    id_cliente        INT NOT NULL,
    fecha             DATETIME2 NOT NULL,
    forma_pago        VARCHAR(20) NOT NULL,
    subtotal          DECIMAL(18,2) NOT NULL,
    descuento         DECIMAL(18,2) NOT NULL DEFAULT 0,
    total             DECIMAL(18,2) NOT NULL,
    id_credito_banco  INT NULL, -- ID del crédito en el sistema del banco (otra BD)
    CONSTRAINT FK_FACTURA_Cliente
        FOREIGN KEY (id_cliente) REFERENCES CLIENTE_COMERCIALIZADORA(id_cliente),
    CONSTRAINT CK_FACTURA_FormaPago
        CHECK (forma_pago IN ('EFECTIVO', 'CREDITO'))
);

/* ---------- TABLA DETALLE_FACTURA ---------- */
CREATE TABLE DETALLE_FACTURA (
    id_detalle      INT IDENTITY(1,1) PRIMARY KEY,
    id_factura      INT NOT NULL,
    id_producto     INT NOT NULL,
    cantidad        INT NOT NULL,
    precio_unitario DECIMAL(18,2) NOT NULL,
    total_linea     DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_DETALLE_FACTURA_Factura
        FOREIGN KEY (id_factura) REFERENCES FACTURA(id_factura),
    CONSTRAINT FK_DETALLE_FACTURA_Producto
        FOREIGN KEY (id_producto) REFERENCES PRODUCTO(id_producto)
);
GO
