using UnityEngine;

public class CameraZoneMove : MonoBehaviour
{
    public float minX = -0.13f;
    public float maxX = 0.13f;
    public float minY = -0.13f;
    public float maxY = 0.13f;
    public float smoothSpeed = 2f; // suavizante de velocidad, mientras menor sea este numero mas lento se moverá al inicio la camara

    private Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position;
    }

    void LateUpdate()
    {
        // Actualizar target según input
        if (Input.GetAxisRaw("Horizontal") > 0) targetPos.x = maxX;
        else if (Input.GetAxisRaw("Horizontal") < 0) targetPos.x = minX;

        if (Input.GetAxisRaw("Vertical") > 0) targetPos.y = maxY;
        else if (Input.GetAxisRaw("Vertical") < 0) targetPos.y = minY;

        // Limitar el area en la que puede estar la camara
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        // Distancia total
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > 0.0001f)
        {
            // t (factor de interpolación) crece rápido al inicio y se desacelera
            float t = 1f - Mathf.Pow(1f - smoothSpeed * Time.deltaTime, 2f);
            transform.position = Vector3.Lerp(transform.position, targetPos, t);
        }
    }
}