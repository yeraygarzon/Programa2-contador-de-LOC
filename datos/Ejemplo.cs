using System;

// Ejemplo para probar el contador
namespace Demo
{
    /// <summary>Clase de ejemplo.</summary>
    public class Calculadora
    {
        private int total;

        public int Total { get; set; }

        public Calculadora()
        {
            total = 0; // inicio
        }

        /* Suma un valor
           y devuelve el total */
        public int Sumar(int x)
        {
            total += x;
            return total;
        }

        public int Doble(int x) => x * 2;

        public string Url()
        {
            return "http://ejemplo.com"; // la // dentro del texto no es comentario
        }
    }

    public class Mensajes
    {
        public void Saludar()
        {
            Console.WriteLine("Hola");
        }
    }
}
