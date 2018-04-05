using UnityEngine;

public class PlayerStatusViewController : BaseGameUIController
{
    private void Start()
    {
        show(false);
    }

    public void statusButtonClicked()
    {
        show(!onShow);
    }
}