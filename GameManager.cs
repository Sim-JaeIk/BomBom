using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스를 정의합니다.
    public static GameManager Instance { get; private set; }

    // 플레이어 객체 배열입니다.
    public GameObject[] players;

    // 스크립트가 깨어날 때 호출됩니다.
    private void Awake()
    {
        // 싱글톤 인스턴스가 이미 존재하는 경우 현재 객체를 즉시 파괴합니다.
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        // 싱글톤 인스턴스가 없는 경우 현재 객체를 인스턴스로 설정하고, 씬이 변경되어도 파괴되지 않도록 합니다.
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        // 씬이 로드될 때마다 플레이어 배열을 초기화합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드되면 players 배열을 다시 초기화합니다.
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // 승리 상태를 확인하는 함수입니다.
    public void CheckWinState()
    {
        // 살아있는 플레이어의 수를 셉니다.
        int aliveCount = 0;
        // 모든 플레이어 객체를 순회합니다.
        foreach (GameObject player in players)
        {
            // 활성화된 플레이어 객체가 있으면 카운트를 증가시킵니다.
            if (player.activeSelf)
            {
                aliveCount++;
            }
        }
        // 살아있는 플레이어가 1명 이하인 경우, 3초 후에 새로운 라운드를 시작합니다.
        if (aliveCount <= 1)
        {
            Invoke(nameof(NewRound), 3f);
        }
    }

    // 새로운 라운드를 시작하는 함수입니다.
    private void NewRound()
    {
        // 현재 활성화된 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
