using UnityEngine;
using System.Collections;

public class ModifyBlocks : MonoBehaviour
{

    Vector2 rot;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                hit.collider.gameObject.transform.GetComponent<PlanetChunk>().SetBlock(hit, BlockTypes.typeEmpty, false);

            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RaycastHit hit2;
                if (Physics.Raycast(transform.position, transform.forward, out hit2, 100))
                {
                    hit2.collider.gameObject.transform.GetComponent<PlanetChunk>().SetBlock(hit2, BlockTypes.typeCore, true);
                    

                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                RaycastHit hit3;
                if (Physics.Raycast(transform.position, transform.forward, out hit3, 100))
                {
                    hit3.collider.gameObject.transform.GetComponent<PlanetChunk>().SetBlock(hit3, BlockTypes.typeGlass, true);

                }
            }
        

        rot = new Vector2(
                rot.x + Input.GetAxis("Mouse X") * 3,
                rot.y + Input.GetAxis("Mouse Y") * 3);

        transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

        transform.position += transform.forward * Input.GetAxis("Vertical");
        transform.position += transform.right * Input.GetAxis("Horizontal");
    }
}
