CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `cards_info` (
        `car_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `car_titulo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `car_descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `car_link` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `car_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`car_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `categoria_pregunta` (
        `cat_preg_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `cat_preg_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `cat_preg_descripcion` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`cat_preg_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `categoria_respuesta` (
        `cat_res_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `cat_res_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `cat_res_descripcion` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`cat_res_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `emociones` (
        `emo_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'identificador de la emocion',
        `emo_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'nombre de la emocion',
        `emo_descripcion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'descripcion de la emocion',
        `emo_image` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'url de la imagen de la emocion',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`emo_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `estado_aprendiz` (
        `est_apr_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador del estado',
        `est_apr_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del estado',
        `est_apr_descrip` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Descripcion del estado',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`est_apr_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `nivel_formacion` (
        `niv_for_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador del tipo de formacion',
        `niv_for_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del tipo de formacion',
        `niv_for_descripcion` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Descripcion del tipo de formacion',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`niv_for_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `psicologo` (
        `psi_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'identificador del psicologo',
        `psi_documento` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'documento del psicologo',
        `psi_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del psicologo',
        `psi_apellido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Apellido del psicologo',
        `psi_especialidad` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'especialidad del psicologo',
        `psi_telefono` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Numero de telefono del psicologo',
        `psi_fecha_registro` datetime NULL COMMENT 'Fecha en la que se le hizo el registro',
        `psi_fecha_nac` date NULL COMMENT 'Fecha de nacimiento',
        `psi_direccion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'ubicacion de su oficina de trabajo',
        `psi_correo_institucional` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'correo institucional',
        `psi_correo_personal` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'correo del psicologo',
        `psi_password` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'contraseña del psicologo',
        `psi_firma` blob NULL,
        `psi_estado_registro` enum('activo','inactivo','suspendido') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo' COMMENT 'estado del psicologo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`psi_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `regional` (
        `reg_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'identificador del regional',
        `reg_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'nombre del regional',
        `reg_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`reg_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `pregunta` (
        `preg_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `preg_descripcion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `preg_categoriaFK` int(40) NULL,
        `preg_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`preg_codigo`),
        CONSTRAINT `pregunta_ibfk_1` FOREIGN KEY (`preg_categoriaFK`) REFERENCES `categoria_pregunta` (`cat_preg_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `respuesta` (
        `res_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `res_categoriaFK` int(40) NULL,
        `res_descripcion` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `res_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`res_codigo`),
        CONSTRAINT `respuesta_ibfk_1` FOREIGN KEY (`res_categoriaFK`) REFERENCES `categoria_respuesta` (`cat_res_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `area` (
        `area_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador de la facultad o area',
        `area_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre de la facultad o area',
        `area_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        `area_psic_codFK` int(40) NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`area_codigo`),
        CONSTRAINT `area_ibfk_1` FOREIGN KEY (`area_psic_codFK`) REFERENCES `psicologo` (`psi_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `centro` (
        `cen_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'identificador del centro',
        `cen_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'nombre del centro',
        `cen_direccion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'direccion del centro',
        `cen_reg_codFK` int(40) NULL,
        `cen_codFK` int(40) NULL,
        `cen_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`cen_codigo`),
        CONSTRAINT `centro_ibfk_1` FOREIGN KEY (`cen_reg_codFK`) REFERENCES `regional` (`reg_codigo`),
        CONSTRAINT `centro_ibfk_2` FOREIGN KEY (`cen_codFK`) REFERENCES `centro` (`cen_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `ciudad` (
        `ciu_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `ciu_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `ciu_regionalFK` int(40) NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`ciu_codigo`),
        CONSTRAINT `ciudad_ibfk_1` FOREIGN KEY (`ciu_regionalFK`) REFERENCES `regional` (`reg_codigo`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `programaformacion` (
        `prog_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Codigo o identificador del programa de formacion',
        `prog_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del programa',
        `prog_modalidad` enum('titulada','complementaria') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Modalidad del programa',
        `prog_forma_modalidad` enum('presencial','virtual','a distancia') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Tipo de modalidad de formacion',
        `prog_niv_formFK` int(40) NULL,
        `prog_areaFK` int(40) NULL,
        `prog_centroFK` int(40) NULL,
        `prog_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`prog_codigo`),
        CONSTRAINT `programaformacion_ibfk_1` FOREIGN KEY (`prog_centroFK`) REFERENCES `centro` (`cen_codigo`),
        CONSTRAINT `programaformacion_ibfk_2` FOREIGN KEY (`prog_niv_formFK`) REFERENCES `nivel_formacion` (`niv_for_codigo`),
        CONSTRAINT `programaformacion_ibfk_3` FOREIGN KEY (`prog_areaFK`) REFERENCES `area` (`area_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `aprendiz` (
        `apr_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Codigo unico del aprendiz',
        `apr_fechacreacion` datetime NOT NULL,
        `apr_tipo_documento` enum('CC','TI','CE') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Tipo de documento de identidad',
        `apr_nro_documento` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Numero de documento',
        `apr_fechaNac` date NULL,
        `apr_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del aprendiz',
        `apr_segundo_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Segundo nombre del aprendiz',
        `apr_apellido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Apellido del aprendiz',
        `apr_segundo_apellido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Segundo Apellido',
        `apr_correo_institucional` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_correo_personal` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Correo del aprendiz',
        `apr_password` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_direccion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_ciudadFK` int(40) NULL,
        `apr_telefono` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Numero de celular',
        `apr_eps` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_patologia` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_estado_aprFK` int(40) NULL,
        `apr_tipo_poblacion` enum('Desplazado','Negro','Discapacitado','Campesino','Afro','Gitano','Indigena','Ninguno') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `apr_telefono_acudiente` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Numero de celular de un acudiente',
        `apr_acud_nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Nombre del acudiente',
        `apr_acud_apellido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'apelldio del acudiente',
        `apr_firma` blob NULL,
        `apr_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo' COMMENT 'Estado del registro',
        `apr_fecha_eliminacion` datetime NULL,
        `apr_razon_eliminacion` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`apr_codigo`),
        CONSTRAINT `aprendiz_ibfk_1` FOREIGN KEY (`apr_estado_aprFK`) REFERENCES `estado_aprendiz` (`est_apr_codigo`) ON DELETE SET NULL,
        CONSTRAINT `aprendiz_ibfk_2` FOREIGN KEY (`apr_ciudadFK`) REFERENCES `ciudad` (`ciu_codigo`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `ficha` (
        `fic_codigo` int(40) NOT NULL COMMENT 'Numero de ficha',
        `fic_jornada` enum('diurna','nocturna','madrugada','mixta') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Tipo de jornada de la ficha',
        `fic_fecha_inicio` date NULL,
        `fic_fecha_fin` date NULL,
        `fic_estado_formacion` enum('en ejecucion','cancelada','terminada por fecha','terminada') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Estado de formacion de la ficha',
        `fic_programaFK` int(40) NULL,
        `fic_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`fic_codigo`),
        CONSTRAINT `ficha_ibfk_1` FOREIGN KEY (`fic_programaFK`) REFERENCES `programaformacion` (`prog_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `diario` (
        `dia_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `dia_titulo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `dia_fecha_creacion` date NULL,
        `dia_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'activo',
        `dia_aprendizFK` int(40) NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`dia_codigo`),
        CONSTRAINT `diario_ibfk_1` FOREIGN KEY (`dia_aprendizFK`) REFERENCES `aprendiz` (`apr_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `verificationcode` (
        `id` int(11) NOT NULL AUTO_INCREMENT,
        `AprendizId` int(11) NULL,
        `PsicologoId` int(11) NULL,
        `Codigo` varchar(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
        `Expiration` datetime NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
        CONSTRAINT `verificationcode_ibfk_1` FOREIGN KEY (`AprendizId`) REFERENCES `aprendiz` (`apr_codigo`),
        CONSTRAINT `verificationcode_ibfk_2` FOREIGN KEY (`PsicologoId`) REFERENCES `psicologo` (`psi_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `aprendiz_ficha` (
        `apr_fic_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `apr_fic_aprendizFK` int(40) NULL,
        `apr_fic_fichaFK` int(40) NULL,
        `apr_fic_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`apr_fic_codigo`),
        CONSTRAINT `aprendiz_ficha_ibfk_1` FOREIGN KEY (`apr_fic_fichaFK`) REFERENCES `ficha` (`fic_codigo`),
        CONSTRAINT `aprendiz_ficha_ibfk_2` FOREIGN KEY (`apr_fic_aprendizFK`) REFERENCES `aprendiz` (`apr_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `pagina_diario` (
        `pag_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `pag_titulo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `pag_contenido` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `pag_fecha_realizacion` datetime NOT NULL,
        `pag_diarioFK` int(40) NULL,
        `pag_emocionFK` int(40) NULL,
        `pag_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`pag_codigo`),
        CONSTRAINT `pagina_diario_ibfk_1` FOREIGN KEY (`pag_emocionFK`) REFERENCES `emociones` (`emo_codigo`),
        CONSTRAINT `pagina_diario_ibfk_2` FOREIGN KEY (`pag_diarioFK`) REFERENCES `diario` (`dia_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `citas` (
        `cit_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador de la cita',
        `cit_tipo_cita` enum('videollamada','chat','presencial') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Tipo de cita',
        `cit_fecha_programada` date NULL COMMENT 'Fecha en la que fue programada la cita',
        `cit_hora_inicio` time NULL COMMENT 'Hora de inicio de la cita',
        `cit_hora_fin` time NULL COMMENT 'Hora de fin de la cita',
        `cit_motivo_solicitud` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `cit_motivo` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Motivo de la cita',
        `cit_anotaciones` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Anotaciones u observaciones antes o despues de la atencio',
        `cit_fecha_creacion` datetime NULL COMMENT 'fecha en la que se realizo el registro de la cita',
        `cit_estado_cita` enum('programada','realizada','cancelada','reprogramada','no asistió') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'estado de la cita',
        `cit_apr_codFK` int(40) NULL COMMENT 'Aprendiz',
        `cit_psi_codFK` int(40) NULL COMMENT 'Psicologo',
        `cit_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`cit_codigo`),
        CONSTRAINT `citas_ibfk_2` FOREIGN KEY (`cit_psi_codFK`) REFERENCES `psicologo` (`psi_codigo`),
        CONSTRAINT `citas_ibfk_3` FOREIGN KEY (`cit_apr_codFK`) REFERENCES `aprendiz_ficha` (`apr_fic_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `seguimiento_aprendiz` (
        `seg_codigo` int(40) NOT NULL AUTO_INCREMENT COMMENT 'Identificador del seguimiento',
        `seg_aprendizFK` int(40) NULL COMMENT 'Aprendiz',
        `seg_psicologoFK` int(40) NULL COMMENT 'Psicologo',
        `seg_fecha_seguimiento` datetime(6) NULL COMMENT 'Fecha de inicio del seguimiento',
        `seg_fecha_fin` datetime(6) NULL COMMENT 'Fecha final del seguimiento',
        `seg_area_remitido` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `seg_trimestre_actual` int(11) NULL,
        `seg_motivo` varchar(400) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `seg_descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'Descripcion u observacion al aprendiz',
        `seg_recomendaciones` varchar(400) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL COMMENT 'recomendaciones para el aprendiz',
        `seg_firma_profesional` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `seg_firma_aprendiz` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `seg_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`seg_codigo`),
        CONSTRAINT `seguimiento_aprendiz_ibfk_3` FOREIGN KEY (`seg_psicologoFK`) REFERENCES `psicologo` (`psi_codigo`),
        CONSTRAINT `seguimiento_aprendiz_ibfk_4` FOREIGN KEY (`seg_aprendizFK`) REFERENCES `aprendiz_ficha` (`apr_fic_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `test_general` (
        `test_gen_codigo` int(40) NOT NULL AUTO_INCREMENT,
        `test_gen_apreFK` int(40) NULL,
        `test_gen_psicoFK` int(40) NULL,
        `test_gen_fecha_realiz` datetime NULL,
        `test_gen_resultados` varchar(400) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `test_gen_recomendacion` varchar(400) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
        `test_gen_estado` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`test_gen_codigo`),
        CONSTRAINT `test_general_ibfk_1` FOREIGN KEY (`test_gen_apreFK`) REFERENCES `aprendiz_ficha` (`apr_fic_codigo`) ON DELETE SET NULL,
        CONSTRAINT `test_general_ibfk_2` FOREIGN KEY (`test_gen_psicoFK`) REFERENCES `psicologo` (`psi_codigo`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE TABLE `test_pregunta` (
        `tes_registroFK` int(40) NOT NULL,
        `tes_pregFK` int(40) NOT NULL,
        `tes_respFK` int(40) NOT NULL,
        `tes_estado_registro` enum('activo','inactivo') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT 'activo',
        CONSTRAINT `PRIMARY` PRIMARY KEY (`tes_registroFK`, `tes_pregFK`, `tes_respFK`),
        CONSTRAINT `test_pregunta_ibfk_1` FOREIGN KEY (`tes_pregFK`) REFERENCES `pregunta` (`preg_codigo`),
        CONSTRAINT `test_pregunta_ibfk_2` FOREIGN KEY (`tes_respFK`) REFERENCES `respuesta` (`res_codigo`),
        CONSTRAINT `test_pregunta_ibfk_3` FOREIGN KEY (`tes_registroFK`) REFERENCES `test_general` (`test_gen_codigo`)
    ) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `apr_ciudadFK` ON `aprendiz` (`apr_ciudadFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `apr_estado_aprFK` ON `aprendiz` (`apr_estado_aprFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `apr_fic_aprendizFK` ON `aprendiz_ficha` (`apr_fic_aprendizFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `apr_fic_codigo` ON `aprendiz_ficha` (`apr_fic_codigo`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `apr_fic_fichaFK` ON `aprendiz_ficha` (`apr_fic_fichaFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `area_psic_codFK` ON `area` (`area_psic_codFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `cen_codFK` ON `centro` (`cen_codFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `cen_reg_codFK` ON `centro` (`cen_reg_codFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `cit_apr_codFK` ON `citas` (`cit_apr_codFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `cit_psi_codFK` ON `citas` (`cit_psi_codFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `ciu_regionalFK` ON `ciudad` (`ciu_regionalFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `dia_aprendizFK` ON `diario` (`dia_aprendizFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `fic_programaFK` ON `ficha` (`fic_programaFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `pag_diarioFK` ON `pagina_diario` (`pag_diarioFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `pag_emocionFK` ON `pagina_diario` (`pag_emocionFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `preg_categoriaFK` ON `pregunta` (`preg_categoriaFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `prog_areaFK` ON `programaformacion` (`prog_areaFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `prog_centroFK` ON `programaformacion` (`prog_centroFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `prog_niv_formFK` ON `programaformacion` (`prog_niv_formFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `res_categoriaFK` ON `respuesta` (`res_categoriaFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `seg_psicologoFK` ON `seguimiento_aprendiz` (`seg_psicologoFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `seguimiento_aprendiz_ibfk_2` ON `seguimiento_aprendiz` (`seg_aprendizFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `reg_test_apreFK` ON `test_general` (`test_gen_apreFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `test_gen_psicoFK` ON `test_general` (`test_gen_psicoFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `tes_pregFK` ON `test_pregunta` (`tes_pregFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `tes_registroFK` ON `test_pregunta` (`tes_registroFK`, `tes_pregFK`, `tes_respFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `tes_respFK` ON `test_pregunta` (`tes_respFK`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `AprendizId` ON `verificationcode` (`AprendizId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    CREATE INDEX `PsicologoId` ON `verificationcode` (`PsicologoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260301182743_AddPsicologoVerificationCode') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260301182743_AddPsicologoVerificationCode', '8.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

