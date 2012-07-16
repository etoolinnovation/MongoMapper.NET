namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    using System;

    public class DeleteDocumentException : Exception
    {
        #region Constants and Fields

        private readonly string _message;

        #endregion

        #region Constructors and Destructors

        public DeleteDocumentException(string message)
        {
            this._message = message;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get
            {
                return String.Format("Error Deleting Document: {0}", this._message);
            }
        }

        #endregion
    }
}