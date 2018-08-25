using System;

namespace CoreSpatial.DbfOper
{
    internal class FieldTypeTransform
    {
        public static Type DbfFieldType2CSharpType(DbfFieldInfo dbfFieldInfo)
        {
            Type cSharpType;
            switch (dbfFieldInfo.FieldType)
            {
                case DbfFieldType.Character:
                    cSharpType = typeof(string);
                    break;

               case DbfFieldType.Numeric:
                    if (dbfFieldInfo.Accuracy == 0)
                    {
                        if (dbfFieldInfo.FieldLength == 4)
                        {
                            cSharpType = typeof(int);
                        }
                        else if(dbfFieldInfo.FieldLength <=8)
                        {
                            cSharpType = typeof(long);
                        }
                        else
                        {
                            cSharpType = typeof(double);
                        }
                    }
                    else
                    {
                        cSharpType = dbfFieldInfo.Accuracy > 6 ? typeof(double) : typeof(float);
                    }
                    break;
                case DbfFieldType.Date:
                    cSharpType = typeof(DateTime);
                    break;

                case DbfFieldType.Logical:
                    cSharpType = typeof(bool);
                    break;

                case DbfFieldType.General:
                case DbfFieldType.Byte:
                default:
                    cSharpType = typeof(byte[]);
                    break;
            }
            return cSharpType;
        }

        public static DbfFieldType CSharpType2DbfFieldType(Type t)
        {
            DbfFieldType dbfFieldType;
            switch (t.Name)
            {
                case "Int32":
                case "Int64":
                case "Double":
                case "Single"://N
                    dbfFieldType = DbfFieldType.Numeric;
                    break;
                case "String"://C
                    dbfFieldType = DbfFieldType.Character;
                    break;
                case "DateTime"://D
                    dbfFieldType = DbfFieldType.Date;
                    break;
                case "Boolean"://L
                    dbfFieldType = DbfFieldType.Logical;
                    break;
                default://G
                    dbfFieldType = DbfFieldType.General;
                    break;
            }
            return dbfFieldType;
        }
    }
}
