using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Raycast")]
    [SerializeField] private LayerMask monsters;
    [SerializeField] private float rayDistance = 1000f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private bool debugRay = true;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (debugLogs)
        {
            Debug.Log($"MouseController Awake. Camera: {(cam != null ? cam.name : "NULL")}");
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            if (debugLogs)
            {
                Debug.LogWarning("MouseController: Mouse.current is NULL. New Input System does not see mouse.");
            }

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (debugLogs)
            {
                Debug.Log("MouseController: Left mouse button pressed.");
            }

            TryKillMonsterUnderCursor();
        }
    }

    private void TryKillMonsterUnderCursor()
    {
        if (cam == null)
        {
            Debug.LogWarning("MouseController: Camera is not assigned.");
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (debugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 2f);
        }

        if (debugLogs)
        {
            Debug.Log($"MouseController: Ray from camera '{cam.name}', mouse position: {mousePosition}, layer mask value: {monsters.value}");
        }

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, monsters, QueryTriggerInteraction.Collide))
        {
            if (debugLogs)
            {
                Debug.Log(
                    "MouseController: Raycast HIT object: " + hit.transform.name +
                    ", root: " + hit.transform.root.name +
                    ", layer: " + LayerMask.LayerToName(hit.transform.gameObject.layer)
                );
            }

            MobController monster = hit.transform.GetComponentInParent<MobController>();

            if (monster == null)
            {
                monster = hit.transform.GetComponentInChildren<MobController>();
            }

            if (monster != null)
            {
                if (debugLogs)
                {
                    Debug.Log("MouseController: MonsterScript found on: " + monster.gameObject.name + ". Calling Death().");
                }

                monster.Death();
            }
            else
            {
                Debug.LogWarning(
                    "MouseController: Hit object, but MonsterScript was not found. " +
                    "Put MonsterScript on the mob parent object, or make sure collider is inside that parent hierarchy."
                );
            }
        }
        else
        {
            if (debugLogs)
            {
                Debug.LogWarning("MouseController: Raycast did not hit anything on selected Monsters LayerMask.");
            }

            TryDebugRaycastWithoutLayerMask(ray);
        }
    }

    private void TryDebugRaycastWithoutLayerMask(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Collide))
        {
            Debug.LogWarning(
                "MouseController DEBUG: Ray hit something WITHOUT layer mask: " +
                hit.transform.name +
                ", layer: " + LayerMask.LayerToName(hit.transform.gameObject.layer) +
                ". This means your Monsters LayerMask is probably wrong."
            );
        }
        else
        {
            Debug.LogWarning(
                "MouseController DEBUG: Ray did not hit anything even without layer mask. " +
                "This usually means the mob has no Collider, the camera is wrong, or you clicked empty space."
            );
        }
    }
}