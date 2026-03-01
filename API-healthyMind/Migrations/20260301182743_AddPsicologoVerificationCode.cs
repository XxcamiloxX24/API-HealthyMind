using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_healthyMind.Migrations
{
    /// <inheritdoc />
    public partial class AddPsicologoVerificationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cards_info",
                columns: table => new
                {
                    car_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    car_titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    car_descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    car_link = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    car_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.car_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "categoria_pregunta",
                columns: table => new
                {
                    cat_preg_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cat_preg_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cat_preg_descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.cat_preg_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "categoria_respuesta",
                columns: table => new
                {
                    cat_res_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cat_res_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cat_res_descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.cat_res_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "emociones",
                columns: table => new
                {
                    emo_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "identificador de la emocion")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    emo_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "nombre de la emocion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    emo_descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "descripcion de la emocion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    emo_image = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "url de la imagen de la emocion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.emo_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "estado_aprendiz",
                columns: table => new
                {
                    est_apr_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Identificador del estado")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    est_apr_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del estado", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    est_apr_descrip = table.Column<string>(type: "text", nullable: true, comment: "Descripcion del estado", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.est_apr_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "nivel_formacion",
                columns: table => new
                {
                    niv_for_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Identificador del tipo de formacion")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    niv_for_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del tipo de formacion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    niv_for_descripcion = table.Column<string>(type: "text", nullable: true, comment: "Descripcion del tipo de formacion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.niv_for_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "psicologo",
                columns: table => new
                {
                    psi_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "identificador del psicologo")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    psi_documento = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, comment: "documento del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Apellido del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_especialidad = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "especialidad del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_telefono = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Numero de telefono del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_fecha_registro = table.Column<DateTime>(type: "datetime", nullable: true, comment: "Fecha en la que se le hizo el registro"),
                    psi_fecha_nac = table.Column<DateOnly>(type: "date", nullable: true, comment: "Fecha de nacimiento"),
                    psi_direccion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "ubicacion de su oficina de trabajo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_correo_institucional = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "correo institucional", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_correo_personal = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "correo del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "contraseña del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    psi_firma = table.Column<byte[]>(type: "blob", nullable: true),
                    psi_estado_registro = table.Column<string>(type: "enum('activo','inactivo','suspendido')", nullable: true, defaultValueSql: "'activo'", comment: "estado del psicologo", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.psi_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "regional",
                columns: table => new
                {
                    reg_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "identificador del regional")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    reg_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "nombre del regional", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reg_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.reg_codigo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "pregunta",
                columns: table => new
                {
                    preg_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    preg_descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    preg_categoriaFK = table.Column<int>(type: "int(40)", nullable: true),
                    preg_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.preg_codigo);
                    table.ForeignKey(
                        name: "pregunta_ibfk_1",
                        column: x => x.preg_categoriaFK,
                        principalTable: "categoria_pregunta",
                        principalColumn: "cat_preg_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "respuesta",
                columns: table => new
                {
                    res_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    res_categoriaFK = table.Column<int>(type: "int(40)", nullable: true),
                    res_descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    res_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.res_codigo);
                    table.ForeignKey(
                        name: "respuesta_ibfk_1",
                        column: x => x.res_categoriaFK,
                        principalTable: "categoria_respuesta",
                        principalColumn: "cat_res_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "area",
                columns: table => new
                {
                    area_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Identificador de la facultad o area")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    area_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre de la facultad o area", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area_psic_codFK = table.Column<int>(type: "int(40)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.area_codigo);
                    table.ForeignKey(
                        name: "area_ibfk_1",
                        column: x => x.area_psic_codFK,
                        principalTable: "psicologo",
                        principalColumn: "psi_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "centro",
                columns: table => new
                {
                    cen_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "identificador del centro")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cen_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "nombre del centro", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cen_direccion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "direccion del centro", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cen_reg_codFK = table.Column<int>(type: "int(40)", nullable: true),
                    cen_codFK = table.Column<int>(type: "int(40)", nullable: true),
                    cen_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.cen_codigo);
                    table.ForeignKey(
                        name: "centro_ibfk_1",
                        column: x => x.cen_reg_codFK,
                        principalTable: "regional",
                        principalColumn: "reg_codigo");
                    table.ForeignKey(
                        name: "centro_ibfk_2",
                        column: x => x.cen_codFK,
                        principalTable: "centro",
                        principalColumn: "cen_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "ciudad",
                columns: table => new
                {
                    ciu_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ciu_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ciu_regionalFK = table.Column<int>(type: "int(40)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ciu_codigo);
                    table.ForeignKey(
                        name: "ciudad_ibfk_1",
                        column: x => x.ciu_regionalFK,
                        principalTable: "regional",
                        principalColumn: "reg_codigo",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "programaformacion",
                columns: table => new
                {
                    prog_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Codigo o identificador del programa de formacion")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prog_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del programa", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prog_modalidad = table.Column<string>(type: "enum('titulada','complementaria')", nullable: true, comment: "Modalidad del programa", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prog_forma_modalidad = table.Column<string>(type: "enum('presencial','virtual','a distancia')", nullable: true, comment: "Tipo de modalidad de formacion", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prog_niv_formFK = table.Column<int>(type: "int(40)", nullable: true),
                    prog_areaFK = table.Column<int>(type: "int(40)", nullable: true),
                    prog_centroFK = table.Column<int>(type: "int(40)", nullable: true),
                    prog_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.prog_codigo);
                    table.ForeignKey(
                        name: "programaformacion_ibfk_1",
                        column: x => x.prog_centroFK,
                        principalTable: "centro",
                        principalColumn: "cen_codigo");
                    table.ForeignKey(
                        name: "programaformacion_ibfk_2",
                        column: x => x.prog_niv_formFK,
                        principalTable: "nivel_formacion",
                        principalColumn: "niv_for_codigo");
                    table.ForeignKey(
                        name: "programaformacion_ibfk_3",
                        column: x => x.prog_areaFK,
                        principalTable: "area",
                        principalColumn: "area_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "aprendiz",
                columns: table => new
                {
                    apr_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Codigo unico del aprendiz")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    apr_fechacreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    apr_tipo_documento = table.Column<string>(type: "enum('CC','TI','CE')", nullable: true, comment: "Tipo de documento de identidad", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_nro_documento = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, comment: "Numero de documento", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_fechaNac = table.Column<DateOnly>(type: "date", nullable: true),
                    apr_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_segundo_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Segundo nombre del aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Apellido del aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_segundo_apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Segundo Apellido", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_correo_institucional = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_correo_personal = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Correo del aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_direccion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_ciudadFK = table.Column<int>(type: "int(40)", nullable: true),
                    apr_telefono = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, comment: "Numero de celular", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_eps = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_patologia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_estado_aprFK = table.Column<int>(type: "int(40)", nullable: true),
                    apr_tipo_poblacion = table.Column<string>(type: "enum('Desplazado','Negro','Discapacitado','Campesino','Afro','Gitano','Indigena','Ninguno')", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_telefono_acudiente = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, comment: "Numero de celular de un acudiente", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_acud_nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Nombre del acudiente", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_acud_apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "apelldio del acudiente", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_firma = table.Column<byte[]>(type: "blob", nullable: true),
                    apr_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", comment: "Estado del registro", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apr_fecha_eliminacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    apr_razon_eliminacion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.apr_codigo);
                    table.ForeignKey(
                        name: "aprendiz_ibfk_1",
                        column: x => x.apr_estado_aprFK,
                        principalTable: "estado_aprendiz",
                        principalColumn: "est_apr_codigo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "aprendiz_ibfk_2",
                        column: x => x.apr_ciudadFK,
                        principalTable: "ciudad",
                        principalColumn: "ciu_codigo",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "ficha",
                columns: table => new
                {
                    fic_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Numero de ficha"),
                    fic_jornada = table.Column<string>(type: "enum('diurna','nocturna','madrugada','mixta')", nullable: true, comment: "Tipo de jornada de la ficha", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fic_fecha_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    fic_fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    fic_estado_formacion = table.Column<string>(type: "enum('en ejecucion','cancelada','terminada por fecha','terminada')", nullable: true, comment: "Estado de formacion de la ficha", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fic_programaFK = table.Column<int>(type: "int(40)", nullable: true),
                    fic_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.fic_codigo);
                    table.ForeignKey(
                        name: "ficha_ibfk_1",
                        column: x => x.fic_programaFK,
                        principalTable: "programaformacion",
                        principalColumn: "prog_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "diario",
                columns: table => new
                {
                    dia_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dia_titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dia_fecha_creacion = table.Column<DateOnly>(type: "date", nullable: true),
                    dia_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: false, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dia_aprendizFK = table.Column<int>(type: "int(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.dia_codigo);
                    table.ForeignKey(
                        name: "diario_ibfk_1",
                        column: x => x.dia_aprendizFK,
                        principalTable: "aprendiz",
                        principalColumn: "apr_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "verificationcode",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AprendizId = table.Column<int>(type: "int(11)", nullable: true),
                    PsicologoId = table.Column<int>(type: "int(11)", nullable: true),
                    Codigo = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expiration = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "verificationcode_ibfk_1",
                        column: x => x.AprendizId,
                        principalTable: "aprendiz",
                        principalColumn: "apr_codigo");
                    table.ForeignKey(
                        name: "verificationcode_ibfk_2",
                        column: x => x.PsicologoId,
                        principalTable: "psicologo",
                        principalColumn: "psi_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "aprendiz_ficha",
                columns: table => new
                {
                    apr_fic_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    apr_fic_aprendizFK = table.Column<int>(type: "int(40)", nullable: true),
                    apr_fic_fichaFK = table.Column<int>(type: "int(40)", nullable: true),
                    apr_fic_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.apr_fic_codigo);
                    table.ForeignKey(
                        name: "aprendiz_ficha_ibfk_1",
                        column: x => x.apr_fic_fichaFK,
                        principalTable: "ficha",
                        principalColumn: "fic_codigo");
                    table.ForeignKey(
                        name: "aprendiz_ficha_ibfk_2",
                        column: x => x.apr_fic_aprendizFK,
                        principalTable: "aprendiz",
                        principalColumn: "apr_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "pagina_diario",
                columns: table => new
                {
                    pag_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    pag_titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pag_contenido = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pag_fecha_realizacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    pag_diarioFK = table.Column<int>(type: "int(40)", nullable: true),
                    pag_emocionFK = table.Column<int>(type: "int(40)", nullable: true),
                    pag_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.pag_codigo);
                    table.ForeignKey(
                        name: "pagina_diario_ibfk_1",
                        column: x => x.pag_emocionFK,
                        principalTable: "emociones",
                        principalColumn: "emo_codigo");
                    table.ForeignKey(
                        name: "pagina_diario_ibfk_2",
                        column: x => x.pag_diarioFK,
                        principalTable: "diario",
                        principalColumn: "dia_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    cit_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Identificador de la cita")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cit_tipo_cita = table.Column<string>(type: "enum('videollamada','chat','presencial')", nullable: true, comment: "Tipo de cita", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cit_fecha_programada = table.Column<DateOnly>(type: "date", nullable: true, comment: "Fecha en la que fue programada la cita"),
                    cit_hora_inicio = table.Column<TimeOnly>(type: "time", nullable: true, comment: "Hora de inicio de la cita"),
                    cit_hora_fin = table.Column<TimeOnly>(type: "time", nullable: true, comment: "Hora de fin de la cita"),
                    cit_motivo_solicitud = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cit_motivo = table.Column<string>(type: "text", nullable: true, comment: "Motivo de la cita", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cit_anotaciones = table.Column<string>(type: "text", nullable: true, comment: "Anotaciones u observaciones antes o despues de la atencio", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cit_fecha_creacion = table.Column<DateTime>(type: "datetime", nullable: true, comment: "fecha en la que se realizo el registro de la cita"),
                    cit_estado_cita = table.Column<string>(type: "enum('programada','realizada','cancelada','reprogramada','no asistió')", nullable: true, comment: "estado de la cita", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cit_apr_codFK = table.Column<int>(type: "int(40)", nullable: true, comment: "Aprendiz"),
                    cit_psi_codFK = table.Column<int>(type: "int(40)", nullable: true, comment: "Psicologo"),
                    cit_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.cit_codigo);
                    table.ForeignKey(
                        name: "citas_ibfk_2",
                        column: x => x.cit_psi_codFK,
                        principalTable: "psicologo",
                        principalColumn: "psi_codigo");
                    table.ForeignKey(
                        name: "citas_ibfk_3",
                        column: x => x.cit_apr_codFK,
                        principalTable: "aprendiz_ficha",
                        principalColumn: "apr_fic_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "seguimiento_aprendiz",
                columns: table => new
                {
                    seg_codigo = table.Column<int>(type: "int(40)", nullable: false, comment: "Identificador del seguimiento")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    seg_aprendizFK = table.Column<int>(type: "int(40)", nullable: true, comment: "Aprendiz"),
                    seg_psicologoFK = table.Column<int>(type: "int(40)", nullable: true, comment: "Psicologo"),
                    seg_fecha_seguimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "Fecha de inicio del seguimiento"),
                    seg_fecha_fin = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "Fecha final del seguimiento"),
                    seg_area_remitido = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_trimestre_actual = table.Column<int>(type: "int(11)", nullable: true),
                    seg_motivo = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "Descripcion u observacion al aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_recomendaciones = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true, comment: "recomendaciones para el aprendiz", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_firma_profesional = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_firma_aprendiz = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seg_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.seg_codigo);
                    table.ForeignKey(
                        name: "seguimiento_aprendiz_ibfk_3",
                        column: x => x.seg_psicologoFK,
                        principalTable: "psicologo",
                        principalColumn: "psi_codigo");
                    table.ForeignKey(
                        name: "seguimiento_aprendiz_ibfk_4",
                        column: x => x.seg_aprendizFK,
                        principalTable: "aprendiz_ficha",
                        principalColumn: "apr_fic_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "test_general",
                columns: table => new
                {
                    test_gen_codigo = table.Column<int>(type: "int(40)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    test_gen_apreFK = table.Column<int>(type: "int(40)", nullable: true),
                    test_gen_psicoFK = table.Column<int>(type: "int(40)", nullable: true),
                    test_gen_fecha_realiz = table.Column<DateTime>(type: "datetime", nullable: true),
                    test_gen_resultados = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    test_gen_recomendacion = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    test_gen_estado = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.test_gen_codigo);
                    table.ForeignKey(
                        name: "test_general_ibfk_1",
                        column: x => x.test_gen_apreFK,
                        principalTable: "aprendiz_ficha",
                        principalColumn: "apr_fic_codigo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "test_general_ibfk_2",
                        column: x => x.test_gen_psicoFK,
                        principalTable: "psicologo",
                        principalColumn: "psi_codigo",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "test_pregunta",
                columns: table => new
                {
                    tes_registroFK = table.Column<int>(type: "int(40)", nullable: false),
                    tes_pregFK = table.Column<int>(type: "int(40)", nullable: false),
                    tes_respFK = table.Column<int>(type: "int(40)", nullable: false),
                    tes_estado_registro = table.Column<string>(type: "enum('activo','inactivo')", nullable: true, defaultValueSql: "'activo'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tes_registroFK, x.tes_pregFK, x.tes_respFK })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                    table.ForeignKey(
                        name: "test_pregunta_ibfk_1",
                        column: x => x.tes_pregFK,
                        principalTable: "pregunta",
                        principalColumn: "preg_codigo");
                    table.ForeignKey(
                        name: "test_pregunta_ibfk_2",
                        column: x => x.tes_respFK,
                        principalTable: "respuesta",
                        principalColumn: "res_codigo");
                    table.ForeignKey(
                        name: "test_pregunta_ibfk_3",
                        column: x => x.tes_registroFK,
                        principalTable: "test_general",
                        principalColumn: "test_gen_codigo");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "apr_ciudadFK",
                table: "aprendiz",
                column: "apr_ciudadFK");

            migrationBuilder.CreateIndex(
                name: "apr_estado_aprFK",
                table: "aprendiz",
                column: "apr_estado_aprFK");

            migrationBuilder.CreateIndex(
                name: "apr_fic_aprendizFK",
                table: "aprendiz_ficha",
                column: "apr_fic_aprendizFK");

            migrationBuilder.CreateIndex(
                name: "apr_fic_codigo",
                table: "aprendiz_ficha",
                column: "apr_fic_codigo");

            migrationBuilder.CreateIndex(
                name: "apr_fic_fichaFK",
                table: "aprendiz_ficha",
                column: "apr_fic_fichaFK");

            migrationBuilder.CreateIndex(
                name: "area_psic_codFK",
                table: "area",
                column: "area_psic_codFK");

            migrationBuilder.CreateIndex(
                name: "cen_codFK",
                table: "centro",
                column: "cen_codFK");

            migrationBuilder.CreateIndex(
                name: "cen_reg_codFK",
                table: "centro",
                column: "cen_reg_codFK");

            migrationBuilder.CreateIndex(
                name: "cit_apr_codFK",
                table: "citas",
                column: "cit_apr_codFK");

            migrationBuilder.CreateIndex(
                name: "cit_psi_codFK",
                table: "citas",
                column: "cit_psi_codFK");

            migrationBuilder.CreateIndex(
                name: "ciu_regionalFK",
                table: "ciudad",
                column: "ciu_regionalFK");

            migrationBuilder.CreateIndex(
                name: "dia_aprendizFK",
                table: "diario",
                column: "dia_aprendizFK");

            migrationBuilder.CreateIndex(
                name: "fic_programaFK",
                table: "ficha",
                column: "fic_programaFK");

            migrationBuilder.CreateIndex(
                name: "pag_diarioFK",
                table: "pagina_diario",
                column: "pag_diarioFK");

            migrationBuilder.CreateIndex(
                name: "pag_emocionFK",
                table: "pagina_diario",
                column: "pag_emocionFK");

            migrationBuilder.CreateIndex(
                name: "preg_categoriaFK",
                table: "pregunta",
                column: "preg_categoriaFK");

            migrationBuilder.CreateIndex(
                name: "prog_areaFK",
                table: "programaformacion",
                column: "prog_areaFK");

            migrationBuilder.CreateIndex(
                name: "prog_centroFK",
                table: "programaformacion",
                column: "prog_centroFK");

            migrationBuilder.CreateIndex(
                name: "prog_niv_formFK",
                table: "programaformacion",
                column: "prog_niv_formFK");

            migrationBuilder.CreateIndex(
                name: "res_categoriaFK",
                table: "respuesta",
                column: "res_categoriaFK");

            migrationBuilder.CreateIndex(
                name: "seg_psicologoFK",
                table: "seguimiento_aprendiz",
                column: "seg_psicologoFK");

            migrationBuilder.CreateIndex(
                name: "seguimiento_aprendiz_ibfk_2",
                table: "seguimiento_aprendiz",
                column: "seg_aprendizFK");

            migrationBuilder.CreateIndex(
                name: "reg_test_apreFK",
                table: "test_general",
                column: "test_gen_apreFK");

            migrationBuilder.CreateIndex(
                name: "test_gen_psicoFK",
                table: "test_general",
                column: "test_gen_psicoFK");

            migrationBuilder.CreateIndex(
                name: "tes_pregFK",
                table: "test_pregunta",
                column: "tes_pregFK");

            migrationBuilder.CreateIndex(
                name: "tes_registroFK",
                table: "test_pregunta",
                columns: new[] { "tes_registroFK", "tes_pregFK", "tes_respFK" });

            migrationBuilder.CreateIndex(
                name: "tes_respFK",
                table: "test_pregunta",
                column: "tes_respFK");

            migrationBuilder.CreateIndex(
                name: "AprendizId",
                table: "verificationcode",
                column: "AprendizId");

            migrationBuilder.CreateIndex(
                name: "PsicologoId",
                table: "verificationcode",
                column: "PsicologoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards_info");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "pagina_diario");

            migrationBuilder.DropTable(
                name: "seguimiento_aprendiz");

            migrationBuilder.DropTable(
                name: "test_pregunta");

            migrationBuilder.DropTable(
                name: "verificationcode");

            migrationBuilder.DropTable(
                name: "emociones");

            migrationBuilder.DropTable(
                name: "diario");

            migrationBuilder.DropTable(
                name: "pregunta");

            migrationBuilder.DropTable(
                name: "respuesta");

            migrationBuilder.DropTable(
                name: "test_general");

            migrationBuilder.DropTable(
                name: "categoria_pregunta");

            migrationBuilder.DropTable(
                name: "categoria_respuesta");

            migrationBuilder.DropTable(
                name: "aprendiz_ficha");

            migrationBuilder.DropTable(
                name: "ficha");

            migrationBuilder.DropTable(
                name: "aprendiz");

            migrationBuilder.DropTable(
                name: "programaformacion");

            migrationBuilder.DropTable(
                name: "estado_aprendiz");

            migrationBuilder.DropTable(
                name: "ciudad");

            migrationBuilder.DropTable(
                name: "centro");

            migrationBuilder.DropTable(
                name: "nivel_formacion");

            migrationBuilder.DropTable(
                name: "area");

            migrationBuilder.DropTable(
                name: "regional");

            migrationBuilder.DropTable(
                name: "psicologo");
        }
    }
}
