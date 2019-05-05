using UIKit;

namespace Mobile.Solution.iOS
{
    public class DocInteractionController : UIDocumentInteractionControllerDelegate
    {
        private readonly UIViewController _navigationController;

        public DocInteractionController(UIViewController controller)
        {
            _navigationController = controller;
        }

        public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
        {
            return _navigationController;
        }

        public override UIView ViewForPreview(UIDocumentInteractionController controller)
        {
            return _navigationController.View;
        }
    }
}