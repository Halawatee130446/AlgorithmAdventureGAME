using UnityEngine;
using UnityEngine.SceneManagement; // 🟢 สำคัญมาก! ต้องมีบรรทัดนี้ถึงจะสั่งเปลี่ยนซีนได้
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("ใส่ชื่อซีนให้ตรงกับเป๊ะๆ (ระวังตัวพิมพ์เล็ก-ใหญ่)")]
    public string levelSelectSceneName = "LevelSelect"; // ชื่อซีนหน้าเลือกด่าน
    public string lessonSceneName = "KnowledgeLibrary"; // ชื่อซีนหน้าบทเรียน
    public string howToPlaySceneName = "HowToPlay";     // ชื่อซีนหน้าสอนเล่น

    // -----------------------------------------
    // ฟังก์ชันสำหรับปุ่ม LEVELS
    // -----------------------------------------
    public void ClickLevels()
    {
        Debug.Log("กำลังไปหน้า Levels...");
        SceneManager.LoadScene(levelSelectSceneName);
    }

    // -----------------------------------------
    // ฟังก์ชันสำหรับปุ่ม LESSON
    // -----------------------------------------
    public void ClickLesson()
    {
        Debug.Log("กำลังไปหน้า Lesson...");
        SceneManager.LoadScene(lessonSceneName);
    }

    // -----------------------------------------
    // ฟังก์ชันสำหรับปุ่ม HOW TO PLAY
    // -----------------------------------------
    public void ClickHowToPlay()
    {
        Debug.Log("กำลังไปหน้า How To Play...");
        SceneManager.LoadScene(howToPlaySceneName);
    }

    // -----------------------------------------
    // ฟังก์ชันสำหรับปุ่ม QUIT (กากบาทสีแดง)
    // -----------------------------------------
    public void ClickQuit()
    {
        Debug.Log("กดออกเกมแล้ว! (ถ้าเล่นใน Unity Editor มันจะไม่ปิดตัวเองนะ ต้อง Build ก่อน)");
        Application.Quit();
    }
}