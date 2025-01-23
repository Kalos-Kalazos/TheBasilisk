using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para manejar el bot�n UI
using UnityEngine.InputSystem; // Aseg�rate de importar el nuevo sistema de entradas

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Referencia al panel UI de Game Over
    private bool isGameOver = false;

    // Referencia al Input Action Asset (para manejar las teclas, si las tienes)
    public InputActionReference restartAction;  // Acci�n para reiniciar
    public InputActionReference quitAction;     // Acci�n para salir

    // Referencia al bot�n de reiniciar
    public Button restartButton;

    private void OnEnable()
    {
        // Activar las acciones de entrada
        restartAction.action.Enable();
        quitAction.action.Enable();

        // A�adir el evento del bot�n de reiniciar
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void OnDisable()
    {
        // Desactivar las acciones de entrada
        restartAction.action.Disable();
        quitAction.action.Disable();

        // Remover el evento del bot�n de reiniciar
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }

    // Llamar cuando el jugador muere
    public void GameOver()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar el cursor y desbloquearlo
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Mostrar la UI de Game Over
        gameOverUI.SetActive(true);

        // Marcar que el juego est� en estado de Game Over
        isGameOver = true;
    }

    // M�todo para reiniciar el juego
    public void RestartGame()
    {
        if (!isGameOver)
            return;

        // Reanudar el tiempo antes de reiniciar
        Time.timeScale = 1f;

        // Ocultar la UI de Game Over
        gameOverUI.SetActive(false);

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // M�todo para salir del juego
    public void QuitGame()
    {
        if (!isGameOver)
            return;

        // Reanudar el tiempo antes de salir
        Time.timeScale = 1f;

        // Ocultar la UI de Game Over
        gameOverUI.SetActive(false);

        // Mostrar el cursor y desbloquearlo antes de salir
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Cerrar el juego
        Application.Quit();
    }

    private void Update()
    {
        // Verificar si el jugador presiona la acci�n de reiniciar o salir desde el teclado
        if (isGameOver)
        {
            if (restartAction.action.triggered) // Si se presion� la tecla para reiniciar
            {
                RestartGame();
            }

            if (quitAction.action.triggered) // Si se presion� la tecla para salir
            {
                QuitGame();
            }
        }
    }
}
