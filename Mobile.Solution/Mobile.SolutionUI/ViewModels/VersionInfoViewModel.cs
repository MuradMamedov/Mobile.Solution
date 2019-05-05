using System;
using System.IO;
using System.Reflection;
using GalaSoft.MvvmLight;

namespace Mobile.Solution.UI.ViewModels
{
    public class VersionInfoViewModel : ViewModelBase
    {
        private Stream _pdfDocumentStream;

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream PdfDocumentStream
        {
            get
            {
                return _pdfDocumentStream;
            }
            set
            {
                _pdfDocumentStream = value;
                RaisePropertyChanged(nameof(PdfDocumentStream));
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public VersionInfoViewModel()
        {
            var x = typeof(Application).GetTypeInfo();
            _pdfDocumentStream = x.Assembly.GetManifestResourceStream(x.Namespace + ".Resources.VersionInfo.pdf");
        }
    }
}
