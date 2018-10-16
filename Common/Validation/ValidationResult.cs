using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Validation
{
    public sealed class ValidationResult
    {
        #region Fields
        bool _isValid = true;
        List<ValidationResult> _moreResults;
        #endregion

        #region Auto Properties
        public string Name { get; set; }

        public string ValidationMessage { get; set; }
        #endregion

        #region Properties

        public bool IsValid
        {
            get
            {
                if (!_isValid)
                    return _isValid;
                else
                {
                    if(MoreResults != null && MoreResults.Any(r => r.IsValid == false))
                    {
                        _isValid =  false;
                    }
                }

                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        public List<ValidationResult> MoreResults
        {
            get
            {
                _moreResults = _moreResults ?? new List<ValidationResult>();
                return _moreResults;
            }
        }
        #endregion


    }
}
