using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ScheduleParser
{
    public class SchemaToObjectTree
    {
        public static TObject ParseDocument<TObject>(Stream rawStream)
        {
            TObject result = default(TObject);
            XmlReader reader = XmlReader.Create(rawStream, new XmlReaderSettings(){IgnoreComments = true});
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));

            if (serializer.CanDeserialize(reader))
            {
                result = (TObject)serializer.Deserialize(reader);
            }            

            return result;
        }
    }
}
