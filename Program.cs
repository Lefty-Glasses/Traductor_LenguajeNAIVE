/*  Grammar:
 *      - entero keyboard_custom :=  int|var
 *      - entero keyboard_custom :=  int|var + int|var
 *      - entero keyboard_custom :=  int|var - int|var
 *      - salto   (int|var) | <string> | <string>(int|var) => writes the specified value adding 
 *                                                            an escape sequence (\n) at the end
 *      - escribe (int|var) | <string> | <string>(int|var) => writes the specified value w/o adding
 *                                                            the escape sequence.
 *      
 *      - The KW for data type and the var_name must be separated with a whitespace
 *      - Comments must not be written in the same line as another KW
 *      
 *  Tokens:
 *      - id            varName
 *      - asigop        :=
 *      - addop         +
 *      - subop         -
 *      - keyword       entero, escribe, salto
 *      - opencln       (
 *      - closecln      )
 *      - comment       #
 *      
 
        entero n2 := 17

        entero suma := n1 + n2
        entero resta := n2 - n1

        salto(suma)
        escribe(resta)

 */

/* --------------------------------- ALGORITMO ----------------------------------- 
 * [x] Crear una clase para almacenar la información de la variable, como su nombre y valor.
   [x] Crear una tabla de símbolos donde la clave será el nombre de la variable y el valor 
      será una instancia de la clase variable. Implementar metodos Insert y Lookup.
   [x] Analizar cada línea de código y determine su tipo (asignación, operación o impresión).
   [x] Para la asignación, insertar o actualizar la variable en la tabla de símbolos.
   [x] Para la operación, calcular el valor utilizando los operandos y asignarlo a la variable.
   [] Para la impresión, buscar el valor dentro de los paréntesis y mostrarlo en la consola.
   [] Verificar los errores posibles, como la variable no declarada, la duplicación de 
       variables, el tipo de datos inválido y la operación incompleta.
 */

/* ----------------------------------- MAIN -------------------------------------- */
using System.Collections;

string rutaArchivo = @"C:\Users\Esteban\Desktop\TrabajosUAT\Sistemas de Base 1\Unidad 1\Tarea2\naiveCode.txt";
string[] lineas = File.ReadAllLines(rutaArchivo);
int row = 0;
byte error = 0;

Hashtable tabla = new Hashtable();

System.Console.WriteLine("Source:");
ImprimirCodigoFuente(lineas);

System.Console.WriteLine("---------------------------------------");
System.Console.WriteLine("Output:");

foreach (string l in lineas)
{
    AnalizarLinea(l, row);
    row++;

    // si se detecta que un mensaje de error ha aparecido, deneter el analisis.
    if (error == 1) { System.Console.ReadKey(); return; }
}

Console.WriteLine();
System.Console.ReadKey();

/* --------------------------------- METODOS ------------------------------------- */

void ImprimirCodigoFuente(string[] codigo)
{
    int line = 1;
    foreach (string c in codigo)
    {
        Console.WriteLine("\t{0}| {1}", line, c);
        line++;
    }
}

int Insertar(string nombre, int value, int numLinea)
{
    // si ya existe una variable con ese nombre
    if (tabla.Count != 0 && tabla.Contains(nombre.Trim()))
    {
        tabla[nombre] = value; // se actualiza el valor
        return 0;
    }
    else
    {
        // de lo contario se crea uno nuevo
        tabla.Add(nombre.Trim(), value);
        return 1;
    }
}

int Buscar(string key, int numLinea)
{
    if (tabla.Count != 0 && tabla.Contains(key.Trim()))
    {
        int valor = Convert.ToInt32(tabla[key.Trim()]);
        return valor;
    }
    else
    {
        // Error: La variable no está en la tabla
        System.Console.WriteLine("Error en linea {0}: La variable '{1}' no se ha declarado",
                                 numLinea, key);
        error = 1;
        return 0;
    }
}

void AnalizarLinea(string line, int row)
{
    row++;

    // Eliminar cualquier espacio en blanco
    line = line.Trim();

        // Verificar si la línea comienza con "entero"
    if (line.StartsWith("entero"))
    {
        HandleAssignment(line, row);
    }
    // Verificar si la linea esta vacia
    else if (line.Equals(string.Empty))
    {
        return;
    }
    // Verificar si la linea es un comentario
    else if (line.Contains('#'))
    {
        // en caso de que se solape con una palabra clave
        if (line.Contains("entero") || line.Contains("salto") || line.Contains("escribe"))
            // muestra el error
            HasComment(row);

        // Interrumpe el analisis y avanza a la siguiente linea
        return;
    }
    // Verificar si la línea contiene "salto"
    else if (line.Contains("salto"))
    {
        HandleJump(line, row);
    }
    // Verificar si la línea contiene "escribe"
    else if (line.Contains("escribe"))
    {
        HandlePrint(line, row);
    }
    // si no es ninguno de los casos anteriores, significa que se trata de una
    // palabra clave que no esta en nuestra gramatica
    else
    {
        Console.WriteLine("\tError en linea {0} - Comando invalido", row);
        error = 1;
        return;
    }
}

void HandleAssignment(string line, int currentLine)
{
    //Console.WriteLine("\tln {0} - Asignacion", currentLine);

    if (!line.Contains(":="))
    {
        Console.WriteLine("\tError en linea {0} - Falta ':='", currentLine);
        error = 1;
        return;
    }

    string[] partes = line.Split(":=");
    string llave = partes[0].Split("entero")[1].Trim();
    string valor = partes[1].Trim();
    if (llave.Equals(string.Empty) || llave.Equals(" "))
    {
        Console.WriteLine("\tError en linea {0} - Identificador no asignado", currentLine);
        error = 1;
        return;
    }

    // en caso de que sea una suma
    if (valor.Contains('+'))
    {
        string[] operandos = valor.Split('+');
        // se verifica que se hayan declarado los 2 operadores
        if (operandos.Length != 2 || operandos[0].Equals("") || operandos[0].Equals("")
                                 || operandos[1].Equals("") || operandos[1].Equals(""))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta un operador", currentLine);
            error = 1;
            return;
        }

        int op1, op2;

        // verificar si los operadores son numeros o son variables:
        // True: Realiza la conversion al numero y lo asigna a la var del operador correspondiente
        // False: Entra al codigo y busca en la TS que el identificador exista
        if (!int.TryParse(operandos[0], out op1))
        {
            // si encuentra el valor (distinto de NULL)
            if (Buscar(operandos[0], currentLine) != 0 && error == 0)
            {
                // asigna el valor encontrado en la tabla de simbolos
                op1 = Buscar(operandos[0], currentLine);
            }
            else
            {
                error = 1;
                return;
            }

        } // lo mismo va para el 2do operador        
        if (!int.TryParse(operandos[1], out op2))
        {
            if (Buscar(operandos[1], currentLine) != 0 && error == 0)
            {
                op2 = Buscar(operandos[1], currentLine);
            }
            else
            {
                error = 1;
                return;
            }
        }

        int res = op1 + op2;
        Insertar(llave, res, currentLine);

    }
    // en caso de que sea una resta
    else if (valor.Contains('-'))
    {
        string[] operandos = valor.Split('-');
        // se verifica que se hayan declarado los 2 operadores
        if (operandos.Length != 2 || operandos[0].Equals("") || operandos[0].Equals("")
                                 || operandos[1].Equals("") || operandos[1].Equals(""))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta un operador", currentLine);
            error = 1;
            return;
        }

        int op1, op2;

        if (!int.TryParse(operandos[0], out op1))
        {
            // si encuentra el valor (distinto de NULL)
            if (Buscar(operandos[0], currentLine) != 0 && error == 0)
            {
                // asigna el valor encontrado en la tabla de simbolos
                op1 = Buscar(operandos[0], currentLine);
            }
            else
            {
                error = 1;
                return;
            }

        } // lo mismo va para el 2do operador        
        if (!int.TryParse(operandos[1], out op2))
        {
            if (Buscar(operandos[1], currentLine) != 0 && error == 0)
            {
                op2 = Buscar(operandos[1], currentLine);
            }
            else
            {
                error = 1;
                return;
            }
        }

        int res = op1 - op2;
        Insertar(llave, res, currentLine);

    }
    // En caso de que sea un numero entero
    else if (int.TryParse(valor.Trim(), out int v))
    {
        if (Insertar(llave, v, currentLine) == 1) // si lo logra insertar correctamente
        {
            return;
        }
        else // si hubo un error
        {
            error = 1;
            return;
        }
    }
    // En caso de que no sea posible hacer la asignacion
    // como x ejemplo si es una pieza de texto o asi
    else
    {
        Console.WriteLine("\tError en linea {0} - Asignacion invalida", currentLine);
        error = 1;
        return;
    }
}

void HandlePrint(string line, int currentLine)
{
    //Console.WriteLine("\tln {0} - Impresion", currentLine);

    if (line.Contains('<') && line.Contains('('))
    {
        if (!line.Contains('>') || !line.Contains(')'))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta '>'", currentLine);
            error = 1;
            return;
        }
        string cadena = line.Substring(line.IndexOf('<') + 1, line.IndexOf('>') - line.IndexOf('<') - 1);
        string id = line.Substring(line.IndexOf('(') + 1, line.IndexOf(')') - line.IndexOf('(') - 1);

        Console.Write("\t{0}", cadena);

        if(Buscar(id, currentLine) == 0)
        {
            return;
        }
        else
        {
            Console.Write(Buscar(id, currentLine));
        }

    }
    else if (line.Contains('('))
    {
        if (!line.Contains(')'))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta ')'", currentLine);
            error = 1;
            return;
        }

        string id = line.Substring(line.IndexOf('(') + 1, line.IndexOf(')') - line.IndexOf('(') - 1);

        if (Buscar(id, currentLine) == 0)
        {
            return;
        }
        else
        {
            Console.Write("\t" + Buscar(id, currentLine));
        }

    }
    else if (line.Contains('<'))
    {
        if (!line.Contains('>'))
        {
            Console.Write("\tError en linea {0} - Hace falta '>'", currentLine);
            error = 1;
            return;
        }

        string cadena = line.Substring(line.IndexOf('<') + 1, line.IndexOf('>') - line.IndexOf('<') - 1);
        Console.Write("\t{0}", cadena);

    }
    else
    {
        Console.WriteLine("\tError en linea {0} - Hace falta '(' o '<'", currentLine);
        error = 1;
        return;
    }

}

void HandleJump(string line, int currentLine)
{
    //Console.WriteLine("\tln {0} - Impresion con salto de linea", currentLine);

    if (line.Contains('<') && line.Contains('('))
    {
        if (!line.Contains('>') || !line.Contains(')'))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta '>'", currentLine);
            error = 1;
            return;
        }
        string cadena = line.Substring(line.IndexOf('<') + 1, line.IndexOf('>') - line.IndexOf('<') - 1);
        string id = line.Substring(line.IndexOf('(') + 1, line.IndexOf(')') - line.IndexOf('(') - 1);

        Console.Write("\t{0}", cadena);

        if (Buscar(id, currentLine) == 0)
        {
            return;
        }
        else
        {
            Console.Write(Buscar(id, currentLine) + "\n");
        }

    }
    else if (line.Contains('('))
    {
        if (!line.Contains(')'))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta ')'", currentLine);
            error = 1;
            return;
        }

        string id = line.Substring(line.IndexOf('(') + 1, line.IndexOf(')') - line.IndexOf('(') - 1);

        if (Buscar(id, currentLine) == 0)
        {
            return;
        }
        else
        {
            Console.Write("\t" + Buscar(id, currentLine) + "\n");
        }

    }
    else if (line.Contains('<'))
    {
        if (!line.Contains('>'))
        {
            Console.WriteLine("\tError en linea {0} - Hace falta '>'", currentLine);
            error = 1;
            return;
        }

        string cadena = line.Substring(line.IndexOf('<') + 1, line.IndexOf('>') - line.IndexOf('<') - 1);
        Console.Write("\t{0}\n", cadena);

    }
    else
    {
        Console.WriteLine("\tError en linea {0} - Hace falta '(' o '<'", currentLine);
        error = 1;
        return;
    }
}

void HasComment(int currentLine)
{
    Console.WriteLine("\tError en linea {0} - Los comentarios deben escribirse en su propia linea", currentLine);
    error = 1;
}
