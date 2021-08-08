using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Lexipun.DotNetFramework.DataProcessing
{
    public class PropertyProcessing<T> : IEnumerator<object>, IEnumerable
    {
        PropertyInfo[] _properties;
        readonly T _sourceObject;
        int _propertyPosition;
        const int _startPoint = -1;
        readonly int countOfProperties;
        private string dateTimeFormat;

        public object SourceObject { get { return _sourceObject; } }
        public object Current { get => GetCurrent(); }
        public int GetCurrentPosition { get => _propertyPosition; }
        public int CountOfProperties { get => countOfProperties; }
        public string DateTimeFormat { get => dateTimeFormat; set => dateTimeFormat = value; }

        public PropertyProcessing(T obj)
        {
            _properties = obj.GetType().GetProperties();
            _sourceObject = obj;
            _propertyPosition = _startPoint;
            countOfProperties = _properties.Length;
            dateTimeFormat = "M/dd/yyyy";
        }



        //Moving
        public bool MoveNext()
        {
            if (_propertyPosition + 1 < _properties.Length)
            {
                ++_propertyPosition;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Previous()
        {
            if (_propertyPosition - 1 > _startPoint)
            {
                --_propertyPosition;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool MoveTo(int where)
        {

            if (where < _properties.Length && where >= _startPoint)
            {
                _propertyPosition = where;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void MoveToEnd()
        {
            _propertyPosition = _properties.Length - 1;
        }
        public void Reset()
        {
            _propertyPosition = _startPoint;
        }


        //basic setter
        public bool SetInRange(int from, int to, params object[] values)

        {

            int indexOfValue = _startPoint + 1;
            for (; (from < to) && (from < _properties.Length) && (indexOfValue < values.Length); ++from)
            {
                if (_properties[from].CanWrite && _properties[from].SetMethod.IsPublic)
                {
                    if (!(values[indexOfValue] is null))
                    {
                        if (_properties[from].PropertyType == values[indexOfValue].GetType()
                            || Nullable.GetUnderlyingType(_properties[from].PropertyType) == values[indexOfValue].GetType()
                            || (_properties[from].PropertyType == typeof(string)))
                        {
                            _properties[from].SetValue(_sourceObject, values[indexOfValue]);
                        }
                        else if (_properties[from].PropertyType.IsEnum)
                        {
                            if (Double.TryParse(values[indexOfValue].ToString(), out _))
                            {
                                var tempNewNumber = Convert.ChangeType(values[indexOfValue], _properties[from].PropertyType.GetEnumUnderlyingType());
                                _properties[from].SetValue(_sourceObject, Enum.Format(_properties[from].PropertyType, tempNewNumber, "g"));
                            }
                            else
                            {
                                _properties[from].SetValue(_sourceObject, Enum.Parse(_properties[from].PropertyType,
                                    values[indexOfValue].ToString()));
                            }
                        }
                        else if (_properties[from].PropertyType.IsClass)
                        {
                            var tempIncludingObject = _properties[from].GetValue(_sourceObject);
                            if (!SetInIncludingClassRange(ref from, ref indexOfValue, to, ref tempIncludingObject, values))
                            {
                                return false;
                            }
                            _properties[from].SetValue(_sourceObject, tempIncludingObject);
                        } else if (_properties[from].PropertyType == typeof(DateTime))
                        {
                            DateTime dateTime;

                            if (!DateTime.TryParseExact(values[indexOfValue].ToString(), dateTimeFormat , CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                            {

                                try
                                {
                                    dateTime = Convert.ToDateTime(values[indexOfValue].ToString());
                                    continue;
                                }
                                catch (FormatException)
                                {
                                    return false;
                                }

                            }

                            _properties[from].SetValue(_sourceObject, dateTime);

                        }
                        else
                        {
                            const char dot = '.';
                            const char coma = ',';
                            string updatedValueForParseToDouble = values[indexOfValue].ToString();

                            if (!Decimal.TryParse(updatedValueForParseToDouble, out decimal tempNumber))
                            {
                                updatedValueForParseToDouble = values[indexOfValue].ToString().Replace(dot, coma);

                                if (!Decimal.TryParse(updatedValueForParseToDouble, out tempNumber))
                                {
                                    return false;
                                }
                            }

                            try
                            {
                                _properties[from].SetValue(_sourceObject, Convert.ToDecimal(tempNumber));
                            }
                            catch (ArgumentException)
                            {
                                _properties[from].SetValue(_sourceObject, Convert.ToUInt32(tempNumber));
                                continue;
                            }

                        }


                        if (!(_properties[from].GetValue(_sourceObject) is null))
                        {
                            if (String.IsNullOrEmpty(_properties[from].GetValue(_sourceObject).ToString()))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            _properties[from].SetValue(_sourceObject, values[indexOfValue]);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }

                    ++indexOfValue;
                }
            }

            return true;
        }

        //Aditional setter for class/struct that in this object
        public bool SetInIncludingClassRange(ref int from, ref int indexOfValues, int to, ref object includingObject, params object[] values)
        {
            int localIndex = 0;
            PropertyInfo[] tempProperties = includingObject.GetType().GetProperties();

            for (; (localIndex < to) && (localIndex < tempProperties.Length) && (localIndex < values.Length); ++localIndex)
            {

                if (_properties[localIndex].CanWrite)
                {
                    Type typeOfCurrentProperty = tempProperties[localIndex].PropertyType;

                    if (typeOfCurrentProperty == values[indexOfValues].GetType() || typeOfCurrentProperty == typeof(string))
                    {
                        tempProperties[localIndex].SetValue(includingObject, values[indexOfValues].ToString());
                    }
                    else if (typeOfCurrentProperty.IsEnum)
                    {
                        tempProperties[localIndex].SetValue(includingObject, Enum.Format(tempProperties[localIndex].PropertyType, values[indexOfValues], "g"));
                    }
                    else if (typeOfCurrentProperty.IsClass)
                    {
                        var tempIncludingObject = tempProperties[localIndex].GetValue(includingObject);
                        if (!SetInIncludingClassRange(ref from, ref indexOfValues, to, ref tempIncludingObject, values))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    if (String.IsNullOrEmpty(tempProperties[localIndex].GetValue(includingObject).ToString()))
                    {
                        return false;
                    }

                    ++indexOfValues;
                }
            }
            --indexOfValues;
            //from += localIndex + 1;
            return true;
        }

        //basic getter
        public object[] GetInRange(ref int from, int to)
        {
            object[] objects = new object[to];
            for (; from < to; ++from)
            {
                objects[from] = Get(from);
            }
            return objects;
        }

        //Setters
        public bool Set(int indexOfProperty, object value)
        {
            if ((indexOfProperty < _properties.Length)
                && _properties[indexOfProperty].CanWrite
                && (_properties[indexOfProperty].PropertyType == value.GetType()))
            {
                _properties[indexOfProperty].SetValue(_sourceObject, value);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetCurrent(object value)
        {
            if (SetInRange(_propertyPosition, _propertyPosition + 1, value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetCurrentAndNext(object value)
        {
            if (SetInRange(_propertyPosition, _propertyPosition++ + 1, value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetAll(params object[] values)
        {
            int BeginFromZero = _startPoint;
            return SetInRange(BeginFromZero, _properties.Length, values);

        }
        public bool SetFrom(ref int from, params object[] values)
        {
            return SetInRange(from, _properties.Length, values);
        }
        public bool SetFromCurrent(params object[] values)
        {
            return SetInRange(_propertyPosition, _properties.Length, values);
        }
        public bool SetTo(int to, params object[] values)
        {
            int BeginFromZero = _startPoint;
            return SetInRange(BeginFromZero, to, values);
        }
        public bool SetFromCurrentTo(int to, params object[] values)
        {
            return SetInRange(_propertyPosition, to, values);

        }

        //Getters
        public object Get(int indexOfProperty)
        {
            return _properties[indexOfProperty].GetValue(_sourceObject);
        }
        public object GetCurrent()
        {
            return _properties[_propertyPosition].GetValue(_sourceObject);
        }
        public object GetCurrentAndNext()
        {
            return _properties[_propertyPosition++].GetValue(_sourceObject);
        }
        public object GetCurrentOnlyPrimitiveValue()
        {
            Type objectType = _properties[_propertyPosition].PropertyType;

            if (objectType.IsClass && objectType != typeof(string))
            {
                var includingClass = _properties[_propertyPosition].GetValue(_sourceObject);
                PropertyProcessing<object> tempPropertyAutoProcessing = new PropertyProcessing<object>(includingClass);

                return tempPropertyAutoProcessing.GetCurrent();
            }
            else
            {
                return GetCurrent();
            }
        }
        public object[] GetAll()
        {
            int BeginFromZero = _startPoint;
            return GetInRange(ref BeginFromZero, _properties.Length);
        }
        public object[] GetFrom(ref int from)
        {
            return GetInRange(ref from, _properties.Length);
        }
        public object[] GetFromCurrent()
        {
            return GetInRange(ref _propertyPosition, _properties.Length);
        }
        public object[] GetTo(int to)
        {
            int BeginFromZero = _startPoint;
            return GetInRange(ref BeginFromZero, to);
        }
        public object[] GetFromCurrentTo(int to)
        {
            return GetInRange(ref _propertyPosition, to);

        }
        public MethodInfo GetCurrentPropertysLevelOfAccessToSet()
        {
            MethodInfo methodInfo = _properties[_propertyPosition].SetMethod;
            if (methodInfo != null)
            {
                return methodInfo;
            }
            else
            {
                return null;
            }
        }

        public MethodInfo GetCurrentPropertysLevelOfAccessToGet()
        {
            MethodInfo methodInfo = _properties[_propertyPosition].GetMethod;
            if (methodInfo != null)
            {
                return methodInfo;
            }
            else
            {
                return null;
            }
        }

        public Type GetCurrentType()
        {
            return _properties[_propertyPosition].PropertyType;
        }

        public Type GetCurrentPrimitiveTypeWIthFirstIncludingType()
        {
            if (_properties[_propertyPosition].PropertyType.IsClass)
            {
                return GetFirstIncludingPrimitiveType(_properties[_propertyPosition].GetValue(_sourceObject));
            }

            return _properties[_propertyPosition].PropertyType;
        }

        public Type[] GetCurrentPrimitiveTypeWithAllIncludingTypes()
        {
            if (_properties[_propertyPosition].PropertyType.IsClass)
            {
                return GetAllIncludingPrimitiveTypes(_properties[_propertyPosition].GetValue(_sourceObject));
            }

            return new Type[] { _properties[_propertyPosition].PropertyType };
        }
        private Type GetFirstIncludingPrimitiveType(object obj)
        {
            var property = obj.GetType().GetProperties()[0];

            if (_properties[_propertyPosition].PropertyType.IsClass)
            {
                return GetFirstIncludingPrimitiveType(property.GetValue(_sourceObject));
            }

            return property.PropertyType;
        }

        private Type[] GetAllIncludingPrimitiveTypes(object obj)
        {
            PropertyProcessing<object> includingProperties = new PropertyProcessing<object>(obj);
            Type[] typesOfObjects = new Type[includingProperties._properties.Length];
            int indexOfTypes = 0;


            foreach (var item in includingProperties)
            {
                if (item.GetType().IsClass && item.GetType() != typeof(string))
                {
                    Type[] tempTypes = GetAllIncludingPrimitiveTypes(item);
                    Type[] resultingTypes = new Type[tempTypes.Length + indexOfTypes];

                    for (int i = 0; i < indexOfTypes; ++i)
                    {
                        resultingTypes[i] = typesOfObjects[i];
                    }

                    for (int i = 0; i < tempTypes.Length; ++i)
                    {
                        resultingTypes[indexOfTypes + i] = tempTypes[i];
                    }

                    typesOfObjects = resultingTypes;
                }
                else
                {
                    typesOfObjects[indexOfTypes] = item.GetType();
                }
                ++indexOfTypes;
            }

            return typesOfObjects;
        }


        public void Dispose()
        {
            _properties = null;
            _propertyPosition = _startPoint;
        }
        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public string GetCurrentNameOfProperty()
        {
            return _properties[_propertyPosition].Name;
        }

        public string GetNameOfPropertyAt(int indexOfProperty)
        {
            return _properties[indexOfProperty].Name;
        }

        public string[] GetAllNamesOfProperties()
        {
            string[] names = new string[countOfProperties];

            for (int i = 0; i < countOfProperties; ++i)
            {
                names[i] = _properties[i].Name;

            }

            return names;
        }

        public List<CustomAttributeData> GetListOfAtributesOFCurrentClass()
        {
            return _sourceObject.GetType().CustomAttributes.ToList();
        }

        public Attribute GetAttributeOfClass(Type type)
        {
            return _sourceObject.GetType().GetCustomAttribute(type);
        }

        public List<CustomAttributeData> GetListOfAtributesOFCurrentProperty()
        {
            return _properties[GetCurrentPosition].CustomAttributes.ToList();
        }

        public Attribute GetAttributeOfCurrentProperty(Type type)
        {
            return _properties[GetCurrentPosition].GetCustomAttribute(type);
        }
    }
}
