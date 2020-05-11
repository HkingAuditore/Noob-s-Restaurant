public class GameManager : MonoSingleton<GameManager>
{
    public SceneStateManager sceneStateManager;
    public UIManager uiManager;
    public ParticleSystemManager particleSystemManager;
    public SequenceManager sequenceManager;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        sceneStateManager = new SceneStateManager();
        uiManager = new UIManager();
        particleSystemManager = new ParticleSystemManager();
        sequenceManager = new SequenceManager();
    }

    private void Start()
    {
        sceneStateManager.Init();
        uiManager.Init();
        particleSystemManager.Init();

        sequenceManager.LoadSequence("StiredEggAndTomato");
    }

    private void Update()
    {
        sceneStateManager.UpdateState();
        uiManager.UpdateUI();
    }
}