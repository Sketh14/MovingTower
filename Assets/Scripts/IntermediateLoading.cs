namespace Moving_Tower
{
    public class IntermediateLoading : UnityEngine.MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Invoke(nameof(LoadScene), 2f);
        }

        private void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
        }
    }
}