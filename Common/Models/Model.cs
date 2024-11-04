using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace Common.Models
{
    public class Model
    {
        //Muestra el contenido de la entidad
        public string ToString()
        {
            System.Type type = this.GetType();


            StringBuilder toString = new StringBuilder();
            toString.Append(type.Name).Append("[");

            foreach (var item in type.GetProperties())
            {
                var value = item.GetValue(this) ?? "";
                toString.Append(item.Name).Append(":").Append(value).Append(", ");
            }

            toString.Length -= 2;
            toString.Append("]");
            return toString.ToString();
        }
    }
}
