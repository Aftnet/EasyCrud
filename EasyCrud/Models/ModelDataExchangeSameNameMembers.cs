using System.Linq;

namespace EasyCrud.Models
{
    public abstract class ModelDataExchangeSameNameMembers<TRecordType, TIndexType> : ModelDataExchangeBase<TRecordType, TIndexType> where TRecordType : IModel<TIndexType>, new()
    {
        protected string[] FieldNamesRequiredForCreation;
        protected string[] FieldNamesRequiredForEditing;
        protected string[] FieldNamesToSkipWhenInitializingFromModel;
        protected string[] FieldNamesToSkipWhenUpdatingModel;

        protected ModelDataExchangeSameNameMembers()
        {
            FieldNamesRequiredForCreation = new string[] { };
            FieldNamesRequiredForEditing = new string[] { };
            FieldNamesToSkipWhenInitializingFromModel = new string[] { };
            FieldNamesToSkipWhenUpdatingModel = new string[] { };
        }

        public override bool IsValidForCreation()
        {
            return FieldsHaveValues(FieldNamesRequiredForCreation);
        }

        public override bool IsValidForEditing()
        {
            return FieldsHaveValues(FieldNamesRequiredForEditing);
        }

        public override void InitializeFromModel(TRecordType input)
        {
            if(input == null)
            {
                return;
            }

            var inputProperties = input.GetType().GetProperties();
            var objectProperties = GetType().GetProperties();

            foreach (var i in objectProperties)
            {
                if (FieldNamesToSkipWhenInitializingFromModel.Contains(i.Name))
                {
                    continue;
                }

                var source = inputProperties.FirstOrDefault(d => d.Name == i.Name);
                if (source != null)
                {
                    i.SetValue(this, source.GetValue(input));
                }
            }
        }

        public override void UpdateModel(TRecordType input)
        {
            if (input == null)
            {
                return;
            }

            var inputProperties = input.GetType().GetProperties();
            var objectProperties = GetType().GetProperties();

            foreach (var i in objectProperties)
            {
                if (FieldNamesToSkipWhenUpdatingModel.Contains(i.Name))
                {
                    continue;
                }

                var value = i.GetValue(this);
                var destination = inputProperties.FirstOrDefault(d => d.Name == i.Name);
                if (destination != null && value != null)
                {
                    destination.SetValue(input, value);
                }
            }
        }

        private bool FieldsHaveValues(string[] fieldNames)
        {
            if (fieldNames == null)
            {
                return false;
            }

            var objectProperties = GetType().GetProperties();

            foreach (var i in objectProperties)
            {
                if (!fieldNames.Contains(i.Name))
                {
                    continue;
                }

                var value = i.GetValue(this);
                if (value == null)
                {
                    return false;
                }

                var valueAsString = value as string;
                if (valueAsString != null)
                {
                    if (valueAsString == string.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
