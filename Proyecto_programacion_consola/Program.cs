using Proyecto_Programacion;
using Proyecto_programacion_consola.Models;
using System.Data;
using System.Drawing;
using Twilio;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using System.Linq;
using System;

ConexionApi api = new ConexionApi();

Console.WriteLine("*** Login ***");
Console.Write("Ingresa tu nombre de usuario: ");
string usuario = Console.ReadLine();
Console.Write("Ingresa tu contraseña: ");
string contraseña = Console.ReadLine();

DtoUsuario usuario_dto = new DtoUsuario()
{
    Usuario = usuario,
    Contraseña = contraseña
};
var usuario_respuesta = await api.Token(usuario_dto);

if (TokenMaster.Token != null)
{
    Console.WriteLine("Bienvenido");
    int opcion;
    do
    {
        Console.WriteLine("\nMenú Principal");
        Console.WriteLine("1.- Mostrar Informacion Dueños y Mascotas");
        Console.WriteLine("2.- Mostrar Citas");
        Console.WriteLine("3.- Salir\n");

        Console.Write("Ingresa una opción: ");
        string input_principal = Console.ReadLine();

        if (EsNumeroEntero(input_principal, out opcion))
        {

            switch (opcion)
            {
                case 1:
                    Console.WriteLine("*********** DUEÑOS Y MASCOTAS s***********\n\n");

                    List<DtoMostrarDueños> lstDueño = await api.ObtenerDueños();

                    DataTable dtDueño = new DataTable();


                    dtDueño.Columns.Add("ID Dueño");
                    dtDueño.Columns.Add("Nombre Dueño");
                    dtDueño.Columns.Add("Telefono");
                    dtDueño.Columns.Add("Direccion");

                    foreach (DtoMostrarDueños dueño in lstDueño)
                    {
                        DataRow drDueño = dtDueño.NewRow();
                        drDueño["ID Dueño"] = dueño.IdDueño;
                        drDueño["Nombre Dueño"] = dueño.NombreC;
                        drDueño["Telefono"] = dueño.Telefono;
                        drDueño["Direccion"] = dueño.Direccion;
                        dtDueño.Rows.Add(drDueño);
                    }


                    Console.WriteLine("{0,-40} {1,-20} {2} \n", "Nombre Dueño", "Teléfono", "Dirección");
                    foreach (DataRow row in dtDueño.Rows)
                    {
                        int id = Convert.ToInt32(row["ID Dueño"]); // Obtener el ID correspondiente a esta fila

                        Console.WriteLine("{0,-40} {1,-20} {2}", row["Nombre Dueño"], row["Telefono"], row["Direccion"]);

                        List<DtoMostrarMascotas> lstMascotas = await api.MostrarMascota(id);


                        Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10}", "Nombre Mascota", "Sexo", "Especie", "Raza", "Edad", "Color");
                        foreach (var mascota in lstMascotas)
                        {
                            Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10}", mascota.Nombre_mascota, mascota.Sexo_mascota, mascota.Especie_mascota, mascota.Raza_mascota, mascota.Edad_mascota, mascota.Color_mascota);
                        }

                        Console.WriteLine("---------------------------------------------------------------------------------------------------------");
                    }

                    int opcion_mascotas_dueños;
                    do
                    {
                        Console.WriteLine("\nOpciones de Mascotas y Dueño");
                        Console.WriteLine("1.- Ingresar Nuevo Dueño");
                        Console.WriteLine("2.- Ingresar Nueva Mascota");
                        Console.WriteLine("3.- Actualizar Dueño");
                        Console.WriteLine("4.- Actualizar Mascota");
                        Console.WriteLine("5.- Eliminar Dueño");
                        Console.WriteLine("6.- Volver al menu principal \n");

                        Console.Write("Ingresa una opción: ");
                        string input_mascotas_dueño = Console.ReadLine();

                        if (EsNumeroEntero(input_mascotas_dueño, out opcion_mascotas_dueños))
                        {

                            switch (opcion_mascotas_dueños)
                            {
                                case 1:

                                    Console.WriteLine("***********Ingresar Nuevo Dueño***********\n");

                                    DtoAgregarDueño pDueño = new DtoAgregarDueño();
                                    Console.Write("Nombre: ");
                                    pDueño.Nombre = Console.ReadLine();
                                    Console.Write("Apellido Paterno: ");
                                    pDueño.Apellido_paterno = Console.ReadLine();
                                    Console.Write("Apellido Materno: ");
                                    pDueño.Apellido_materno = Console.ReadLine();
                                    Console.Write("Telefono: ");
                                    pDueño.Telefono = Console.ReadLine();
                                    Console.Write("Direcion: ");
                                    pDueño.Direccion = Console.ReadLine();

                                    var resultadoDueño = await api.InsertarDueño(pDueño);

                                    if (string.IsNullOrEmpty(resultadoDueño.Error))
                                    {
                                        Console.WriteLine("\nDueño insertado correctamente\n");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error al insertar dueño: " + resultadoDueño.Error);
                                    }
                                    break;
                                case 2:
                                    Console.WriteLine("***********Ingresar Nueva Mascota***********\n");

                                    Console.WriteLine("Para ingresar una mascota tienes que asignarle su dueño\n");

                                    Console.WriteLine("Dueños Disponibles\n");

                                    foreach (var dueño_id_nombre in lstDueño)
                                    {
                                        Console.WriteLine("{0} - {1}", dueño_id_nombre.IdDueño, dueño_id_nombre.NombreC);
                                    }

                                    Console.WriteLine("\nIngresa un numero de dueño para asignarle su mascota");

                                    int IdDueñoSeleccionado;
                                    bool es_numero = int.TryParse(Console.ReadLine(), out IdDueñoSeleccionado);
                                    if (es_numero)
                                    {
                                        if (lstDueño.Any(d => d.IdDueño == IdDueñoSeleccionado))
                                        {
                                            Console.WriteLine("***********Ingresar Mascota***********\n");

                                            DTOAgregarMascotas pMascotas = new DTOAgregarMascotas();
                                            pMascotas.IdDueño = IdDueñoSeleccionado;
                                            Console.Write("Nombre: ");
                                            pMascotas.Nombre_mascota = Console.ReadLine();
                                            Console.Write("Sexo: ");
                                            pMascotas.Sexo_mascota = Console.ReadLine();
                                            Console.Write("Edad: ");
                                            pMascotas.Edad_mascota = Console.ReadLine();
                                            Console.Write("Especie: ");
                                            pMascotas.Especie_mascota = Console.ReadLine();
                                            Console.Write("Raza: ");
                                            pMascotas.Raza_mascota = Console.ReadLine();
                                            Console.Write("Color: ");
                                            pMascotas.Color_mascota = Console.ReadLine();

                                            var resultado_mascota = await api.InsertarMascotas(pMascotas);

                                            if (string.IsNullOrEmpty(resultado_mascota.Error_mascota))
                                            {
                                                Console.WriteLine("\nMascota insertado correctamente\n");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nError al insertar mascota: " + resultado_mascota.Error_mascota);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("El numero del dueño ingresado no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("El valor ingresado no es un número válido. Intente de nuevo.\n");
                                    }
                                    break;
                                case 3:
                                    Console.WriteLine("***********Actualizar Dueño***********\n");

                                    foreach (var dueño_id_nombre in lstDueño)
                                    {
                                        Console.WriteLine("{0} - {1}", dueño_id_nombre.IdDueño, dueño_id_nombre.NombreC);
                                    }

                                    Console.WriteLine("\nIngresa un numero de dueño para poder actualizarlo");


                                    int IdDueñoSeleccionado_actualizar;
                                    es_numero = int.TryParse(Console.ReadLine(), out IdDueñoSeleccionado_actualizar);
                                    if (es_numero)
                                    {
                                        if (lstDueño.Any(d => d.IdDueño == IdDueñoSeleccionado_actualizar))
                                        {
                                            DtoActualizarDueño pDueño_actualizado = new DtoActualizarDueño();

                                            pDueño_actualizado.IdDueño = IdDueñoSeleccionado_actualizar;
                                            Console.Write("Nombre: ");
                                            pDueño_actualizado.Nombre = Console.ReadLine();
                                            Console.Write("Apellido Paterno: ");
                                            pDueño_actualizado.Apellido_paterno = Console.ReadLine();
                                            Console.Write("Apellido Materno: ");
                                            pDueño_actualizado.Apellido_materno = Console.ReadLine();
                                            Console.Write("Telefono: ");
                                            pDueño_actualizado.Telefono = Console.ReadLine();
                                            Console.Write("Direcion: ");
                                            pDueño_actualizado.Direccion = Console.ReadLine();

                                            var resultadoDueño_actualizado = await api.ActualizarDueño(IdDueñoSeleccionado_actualizar, pDueño_actualizado);

                                            if (string.IsNullOrEmpty(resultadoDueño_actualizado.Error))
                                            {
                                                Console.WriteLine("\nDueño Actualizado Correctamente\n");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nError al actualizar el dueño: " + resultadoDueño_actualizado.Error);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("El ID del dueño ingresado no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("El valor ingresado no es un número válido. Intente de nuevo.\n");
                                    }
                                    break;
                                case 4:
                                    Console.WriteLine("***********Actualizar Mascota***********\n");

                                    foreach (var dueño_id_nombre in lstDueño)
                                    {
                                        Console.WriteLine("{0} - {1}", dueño_id_nombre.IdDueño, dueño_id_nombre.NombreC);
                                    }

                                    Console.WriteLine("\nIngresa un numero de dueño para poder ver sus mascotas");
                                    int IdMascotaSeleccionada_dueño_actualizar;
                                    es_numero = int.TryParse(Console.ReadLine(), out IdMascotaSeleccionada_dueño_actualizar);
                                    if (es_numero)
                                    {
                                        if (lstDueño.Any(d => d.IdDueño == IdMascotaSeleccionada_dueño_actualizar))
                                        {
                                            List<DtoMostrarMascotas> lstMascotas = await api.MostrarMascota(IdMascotaSeleccionada_dueño_actualizar);

                                            foreach (var mascota_id_nombre in lstMascotas)
                                            {
                                                Console.WriteLine("{0} - {1}", mascota_id_nombre.IdMascotas, mascota_id_nombre.Nombre_mascota);
                                            }

                                            Console.WriteLine("\nIngresa un numero de mascota para poder poder actualizarlo (La imagen no se actualizara >=( )");
                                            int idMascota_actualizar;
                                            bool conservion_mascota_act = int.TryParse(Console.ReadLine(), out idMascota_actualizar);
                                            if (conservion_mascota_act)
                                            {
                                                if (lstMascotas.Any(d => d.IdMascotas == idMascota_actualizar))
                                                {
                                                    DtoActualizarMascotas pMascotas_actualizar = new DtoActualizarMascotas();
                                                    pMascotas_actualizar.IdMascotas = idMascota_actualizar;
                                                    Console.Write("Nombre: ");
                                                    pMascotas_actualizar.Nombre_mascota = Console.ReadLine();
                                                    Console.Write("Sexo: ");
                                                    pMascotas_actualizar.Sexo_mascota = Console.ReadLine();
                                                    Console.Write("Edad: ");
                                                    pMascotas_actualizar.Edad_mascota = Console.ReadLine();
                                                    Console.Write("Especie: ");
                                                    pMascotas_actualizar.Especie_mascota = Console.ReadLine();
                                                    Console.Write("Raza: ");
                                                    pMascotas_actualizar.Raza_mascota = Console.ReadLine();
                                                    Console.Write("Color: ");
                                                    pMascotas_actualizar.Color_mascota = Console.ReadLine();

                                                    var resultado_mascota = await api.ActualizarMascotas(pMascotas_actualizar);

                                                    if (string.IsNullOrEmpty(resultado_mascota.Error_mascota))
                                                    {
                                                        Console.WriteLine("\nMascota actualizada correctamente\n");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("\nError al actualizar la mascota: " + resultado_mascota.Error_mascota);
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("El numero de la mascota no existe. Por favor, intente de nuevo.\n");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("El valor ingresado no es un número válido. Intente de nuevo.\n.\n");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("El numero del dueño ingresado no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("El valor ingresado no es un número válido. Intente de nuevo.\n");
                                    }
                                    break;
                                case 5:
                                    Console.WriteLine("***********Eliminar Dueño***********\n");

                                    foreach (var dueño_id_nombre in lstDueño)
                                    {
                                        Console.WriteLine("{0} - {1}", dueño_id_nombre.IdDueño, dueño_id_nombre.NombreC);
                                    }

                                    Console.WriteLine("\nRECUERDA QUE SI ELIMINAS EL DUEÑO TODAS SUS MASCOTAS SE ELIMINARAN");
                                    Console.WriteLine("\nIngresa un numero de dueño para asignarle su mascota");

                                    int id_dueño_eliminar;
                                    es_numero = int.TryParse(Console.ReadLine(), out id_dueño_eliminar);

                                    if (es_numero)
                                    {
                                        if (lstDueño.Any(d => d.IdDueño == id_dueño_eliminar))
                                        {
                                            DtoEliminarDueño eliminarDueño = await api.EliminarDueño(id_dueño_eliminar);

                                            if (eliminarDueño != null)
                                            {
                                                Console.WriteLine("Dueño eliminado correctamente.", "Eliminación exitosa");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error al eliminar dueño");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("El numero del dueño ingresado no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("El valor ingresado no es un número válido. Intente de nuevo.\n.\n");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nSolo se aceptan numeros del menu.\n");
                        }

                    } while (opcion_mascotas_dueños != 6);
                    break;
                case 2:
                    Console.WriteLine("*********** CITAS ***********\n");

                    int opcion_citas;
                    do
                    {
                        Console.WriteLine("\nOpciones de Citas");
                        Console.WriteLine("1.- Mostrar Citas Por Fecha");
                        Console.WriteLine("2.- Ingresar Nueva Cita");
                        Console.WriteLine("3.- Actualizar Cita");
                        Console.WriteLine("4.- Salir Al Menu Principal\n");

                        Console.Write("Ingresa una opción: ");
                        string input_citas = Console.ReadLine();

                        if (EsNumeroEntero(input_citas, out opcion_citas))
                        {
                            switch (opcion_citas)
                            {
                                case 1:
                                    Console.WriteLine("Citas Por Fecha\n");
                                    List<DtoMostrarCitas> lstCitas = await api.MostrarCitas();

                                    var fechasDisponibles = lstCitas.Select(g => g.Fecha_Consulta.ToString("dd/MM/yyyy")).Distinct();

                                    Console.WriteLine("Fechas Disponibles:\n");
                                    foreach (var fecha in fechasDisponibles)
                                    {        
                                        Console.WriteLine(fecha);
                                    }

                                    Console.WriteLine("Ingresa una fecha (DD/MM/AAAA):");
                                    var fechaIngresada = Console.ReadLine();


                                    if (DateTime.TryParseExact(fechaIngresada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaValida))
                                    {
                                        if (fechasDisponibles.Contains(fechaIngresada))
                                        {
                                            Console.WriteLine("\n");
                                            var citas_del_dia = lstCitas.Where(g => g.Fecha_Consulta.ToString("dd/MM/yyyy") == fechaIngresada);

                                            Console.WriteLine("{0,-30} {1,-15} {2,-20} {3,-20} {4,-15} {5}", "DUEÑO", "MASCOTA", "MOTIVO CONSULTA", "FECHA CONSULTA", "HORA", "CONFIRMADA");
                                            foreach (var citas in citas_del_dia)
                                            {
                                                Console.WriteLine($"{citas.Dueño,-30} {citas.Mascota,-15} {citas.Motivo_Consulta,-20} {citas.Fecha_Consulta.ToString("dd/MM/yyyy"),-20} {citas.Hora,-15}" + "{0}", citas.Activo ? "Si" : "No");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("Fecha ingresada no válida. Ingresa una fecha válida.");
                                    }
                                    break;
                                case 2:
                                    Console.WriteLine("\nIngresar Nueva Cita\n");

                                    List<DtoMostrarDueños> lstDueño_citas = await api.ObtenerDueños();

                                    foreach (var dueño_citas in lstDueño_citas)
                                    {
                                        Console.WriteLine($"{dueño_citas.IdDueño} - {dueño_citas.NombreC}");
                                    }

                                    Console.WriteLine("\nIngresa un numero de dueño para poder ver sus mascotas");
                                    int IdDueño_actualizar;
                                    bool es_numero = int.TryParse(Console.ReadLine(), out IdDueño_actualizar);
                                    if (es_numero)
                                    {
                                        string numero_tel = lstDueño_citas.Where(d => d.IdDueño == IdDueño_actualizar).Select(d => d.Telefono).FirstOrDefault();

                                        if (lstDueño_citas.Any(d => d.IdDueño == IdDueño_actualizar))
                                        {
                                            Console.Write("\n");
                                            List<DtoMostrarMascotas> lstmascotas_citas = await api.MostrarMascota(IdDueño_actualizar);

                                            foreach (var mascotas_citas in lstmascotas_citas)
                                            {
                                                Console.WriteLine($"{mascotas_citas.IdMascotas} - {mascotas_citas.Nombre_mascota}");
                                            }

                                            Console.WriteLine("\nIngresa un numero de mascota para poder agendar la cita");
                                            int IdMascota_cita;
                                            es_numero = int.TryParse(Console.ReadLine(), out IdMascota_cita);
                                            if (es_numero)
                                            {
                                                string nombre_mascota = lstmascotas_citas.Where(d => d.IdMascotas == IdMascota_cita).Select(d => d.Nombre_mascota).FirstOrDefault();
                                                if (lstmascotas_citas.Any(d => d.IdMascotas == IdMascota_cita))
                                                {
                                                    DtoAgregarCita pCitas = new DtoAgregarCita();

                                                    pCitas.IdMascotas = IdMascota_cita;
                                                    Console.Write("\nMotivo consulta: ");
                                                    pCitas.Motivo_consulta = Console.ReadLine();
                                                    Console.Write("Fecha consulta: (dd/MM/yyyy) ");
                                                    pCitas.Fecha_consulta = DateTime.Parse(Console.ReadLine());
                                                    Console.Write("Hora: ");
                                                    pCitas.Hora = Console.ReadLine();

                                                    var resultado_cita_agregar = await api.AgregarCita(pCitas);

                                                    if (string.IsNullOrEmpty(resultado_cita_agregar.Error))
                                                    {
                                                        Console.WriteLine("\nCita agregada con exito\n");

                                                        var accountSid = "AC4ffa3bccbc48e62c0881c1f456101bb4";
                                                        var authToken = "667742d1135345d9934d8263bd088164";
                                                        TwilioClient.Init(accountSid, authToken);
                                                        string numero_telefono_sinespacio = numero_tel.Replace("-", "");
                                                        var messageOptions = new CreateMessageOptions(
                                                          new PhoneNumber($"whatsapp:+521{numero_telefono_sinespacio}"));
                                                        messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
                                                        messageOptions.Body = $"La cita de {nombre_mascota} es el dia {pCitas.Fecha_consulta.ToString("dd/MM/yyyy")} a las {pCitas.Hora} por el motivo de {pCitas.Motivo_consulta} ";

                                                        var message = MessageResource.Create(messageOptions);
                                                        Console.WriteLine(message.Body);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("\nError al agregar cita: \n" + resultado_cita_agregar.Error);
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\nEl numero de la mascota ingresada no existe. Intente de nuevo.\n");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nEl valor ingresado no es un número válido. Intente de nuevo.\n");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nEl numero del dueño ingresado no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nEl valor ingresado no es un número válido. Intente de nuevo.\n");

                                    }
                                    break;
                                case 3:
                                    Console.WriteLine("\nActualizar Cita De Hoy\n");
                                    List<DtoMostrarCitas> lstCitas_hoy = await api.MostrarCitas();
                                    DateTime fecha_hoy = DateTime.Today;

                                    if (lstCitas_hoy.Any(d => d.Fecha_Consulta == fecha_hoy))
                                    {
                                        var citas_del_dia = lstCitas_hoy.Where(g => g.Fecha_Consulta.ToString("MM/dd/yyyy") == fecha_hoy.ToString("MM/dd/yyyy"));

                                        Console.WriteLine("{0,-15} {1,-30} {2,-15} {3,-20} {4,-15} {5,-10} {6}", "NUMERO CITA", "DUEÑO", "MASCOTA", "MOTIVO CONSULTA", "FECHA CONSULTA", "HORA", "CONFIRMADA");
                                        foreach (var citas_hoy in citas_del_dia)
                                        {
                                            Console.WriteLine($"{citas_hoy.IdCita,-15} {citas_hoy.Dueño,-30} {citas_hoy.Mascota,-15} {citas_hoy.Motivo_Consulta,-20} {citas_hoy.Fecha_Consulta.ToString("dd/MM/yyyy"),-15} {citas_hoy.Hora,-10}" + "{0}", citas_hoy.Activo ? "Si" : "No");
                                        }
                                    }
                                    Console.WriteLine("\n Selecciona el numero de cita que quieres actualizar");
                                    int id_cita_actualizar;
                                    es_numero = int.TryParse(Console.ReadLine(), out id_cita_actualizar);
                                    if (es_numero)
                                    {
                                        if (lstCitas_hoy.Any(d => d.IdCita == id_cita_actualizar))
                                        {
                                            DtoActualizarCita pCitas_actualizar = new DtoActualizarCita();

                                            pCitas_actualizar.IdCitas = id_cita_actualizar;
                                            Console.Write("\nMotivo consulta: ");
                                            pCitas_actualizar.Motivo_consulta = Console.ReadLine();
                                            Console.Write("Fecha consulta: (dd/MM/yyyy) ");
                                            pCitas_actualizar.Fecha_consulta = DateTime.Parse(Console.ReadLine());
                                            Console.Write("Hora: ");
                                            pCitas_actualizar.Hora = Console.ReadLine();
                                            Console.Write("Confirmada [S/N]: ");
                                            char confirmadaInput = Console.ReadKey().KeyChar;
                                            pCitas_actualizar.Activo = (confirmadaInput == 'S' || confirmadaInput == 's') ? Convert.ToBoolean(1) : Convert.ToBoolean(0);

                                            var resultado_cita_actualizar = await api.ActualizarCita(pCitas_actualizar);

                                            if (resultado_cita_actualizar != null)
                                            {
                                                Console.WriteLine("\nCita actualizada con exito.");
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nError al Actualizar cita" + pCitas_actualizar.Error);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nEl numero de la cita ingresada no existe. Por favor, intenta de nuevo.\n");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nEl valor ingresado no es un número válido. Intente de nuevo.\n");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nSolo se aceptan numeros del menu.\n");
                        }
                    } while (opcion_citas != 4);

                    break;
            }
        }
        else
        {
            Console.WriteLine("Solo se aceptan numeros del menu.\n");
        }
    } while (opcion != 3);
}
else
{
    Console.WriteLine("Usuario o contraseña incorrectos");
}

static bool EsNumeroEntero(string input, out int numero)
{
    return int.TryParse(input, out numero);
}
