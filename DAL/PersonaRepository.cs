
using System.Collections.Generic;
using System.Linq;
using Entity;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Transactions;

namespace DAL
{
    public class PersonaRepository
    {
        private readonly OracleConnection _connection;
        private readonly List<Persona> _personas = new List<Persona>();
        public PersonaRepository(ConnectionManager connection)
        {
            _connection = connection._conexion;
        }


        public int Guardar(Persona persona)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"Insert Into Persona (Identificacion,Nombre,Edad, Sexo, Pulsacion) 
                                      values (:Identificacion,:Nombre,:Edad,:Sexo,:Pulsacion)";
                command.Parameters.Add("Identificacion", OracleDbType.Varchar2).Value = persona.Identificacion;
                command.Parameters.Add("Nombre", OracleDbType.Varchar2).Value = persona.Nombre;
                command.Parameters.Add("Edad", OracleDbType.Int32).Value = persona.Edad;
                command.Parameters.Add("Sexo", OracleDbType.Varchar2).Value = persona.Sexo;
                command.Parameters.Add("Pulsacion", OracleDbType.Decimal).Value = persona.Pulsacion;
                var filas = command.ExecuteNonQuery();
                return filas;
            }
        }

        
        public int Eliminar(Persona persona)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "Delete from persona where Identificacion=:Identificacion";
                command.Parameters.Add("Identificacion", OracleDbType.Varchar2).Value = persona.Identificacion;
                var filas = command.ExecuteNonQuery();
                return filas;
            }
        }

        public List<Persona> ConsultarTodos()
        {
           OracleDataReader dataReader;
            List<Persona> personas = new List<Persona>();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "Select * from persona ";
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        Persona persona = DataReaderMapToPerson(dataReader);
                        personas.Add(persona);
                    }
                }
            }
            return personas;
        }
        public Persona BuscarPorIdentificacion(string identificacion)
        {
            OracleDataReader dataReader;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "select Identificacion,Nombre,Sexo,Edad,Pulsacion from persona where Identificacion=:Identificacion";
                command.Parameters.Add("Identificacion", OracleDbType.Varchar2).Value = identificacion; 
                dataReader = command.ExecuteReader();
                dataReader.Read();
                Persona persona = DataReaderMapToPerson(dataReader);
                return persona;
            }
        }

        public int Modificar(Persona persona)
        {
           using (var command = _connection.CreateCommand())
           {
                command.CommandText = @"update persona set nombre=:Nombre, edad=:Edad, sexo=:Sexo, pulsacion=:Pulsacion
                                        where Identificacion=:Identificacion";
                
                command.Parameters.Add("Nombre", OracleDbType.Varchar2).Value = persona.Nombre;
                command.Parameters.Add("Edad", OracleDbType.Int32).Value = persona.Edad;
                command.Parameters.Add("Sexo", OracleDbType.Varchar2).Value = persona.Sexo;
                command.Parameters.Add("Pulsacion", OracleDbType.Decimal).Value = persona.Pulsacion;
                command.Parameters.Add("Identificacion", OracleDbType.Varchar2).Value = persona.Identificacion;
                OracleTransaction transaction = _connection.BeginTransaction();
                var filas = command.ExecuteNonQuery();
                transaction.Commit();
               return filas;
           }
            
            
        }
        private Persona DataReaderMapToPerson(OracleDataReader dataReader)
        {
            if (!dataReader.HasRows) return null;
            Persona persona = new Persona();
            persona.Identificacion = dataReader.GetString(0);
            persona.Nombre = dataReader.GetString(1);
            persona.Sexo = dataReader.GetString(2);
            persona.Edad = dataReader.GetInt32(3);
            persona.Pulsacion = dataReader.GetDecimal(4);
            return persona;

        }

  
        public int Totalizar()
        {
            
            return ConsultarTodos().Count();
        }
        public int TotalizarTipo(string tipo)
        {
           
            return ConsultarTodos().Where(p => p.Sexo.Equals(tipo)).Count();
        }
      
    }
}
