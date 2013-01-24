using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    public class DeleteDocumentException : Exception
    {
        #region Constants and Fields

        private readonly string _message;

        #endregion

        #region Constructors and Destructors

        public DeleteDocumentException(string message)
        {
            _message = message;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get { return String.Format("Error Deleting Document: {0}", _message); }
        }

        #endregion
    }
}