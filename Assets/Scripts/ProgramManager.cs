using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour {
    public int discCount;
    public Disc discPrefab;
    public Pole[] poles;
    public float maxDiscWidth = 2f;
    public float minDiscWidth = 0.5f;
    public Gradient discColors;
    public List<Disc> discs;
    private float discHeight;
    public bool gaming = false;

    private void Start() {
        discs = new List<Disc>();
    }

    public void Prepare(Slider slider) {
        this.discCount = (int)slider.value;
        discHeight = 2f / discCount;
        for(int i = discCount - 1 ; i >= 0 ; i--) {
            Disc disc = Instantiate<Disc>(discPrefab);
            disc.transform.position = poles[0].transform.position + poles[0].discLocation + Vector3.up * (discCount - i - 1) * discHeight;
            float radius;
            if (discCount != 1) {
                radius = Mathf.Lerp(minDiscWidth, maxDiscWidth, i / (discCount - 1f));
            } else {
                radius = Mathf.Lerp(minDiscWidth, maxDiscWidth, 0f);
            }
            disc.transform.localScale = new Vector3(radius, discHeight, radius);
            MeshRenderer discMeshRenderer = disc.GetComponentInChildren<MeshRenderer>();
            Material discMaterial = new Material(discMeshRenderer.material);
            float colorTime;
            if (discCount != 1) {
                colorTime = (float)(discCount - i - 1) / (discCount - 1);
            }
            else {
                colorTime = 1;
            }
            discMaterial.color = discColors.Evaluate(colorTime);
            discMeshRenderer.material = discMaterial;

            poles[0].discs.Push(disc);
            discs.Add(disc);
            disc.index = i;
            disc.pole = poles[0];
        }
        discs.Reverse();
    }

    public void Reset() {
        StopAllCoroutines();
        foreach(Disc disc in discs) {
            Destroy(disc.gameObject);
        }
        foreach(Pole pole in poles) {
            pole.discs.Clear();
        }
        discs.Clear();
        gaming = false;
    }

    public void Solve() {
        if(discCount > 0 && !gaming) {
            StartCoroutine(SolveAnim(discCount));
        }
    }

    IEnumerator SolveAnim(int discCount) {
        gaming = true;
        int k = discCount;
        while (k > 0) {
            yield return StartCoroutine(MoveDisc(--k));
        }
        gaming = false;
    }

    IEnumerator MoveDisc(int index) {
        int k = index;
        while (k > 0)
        {
            yield return StartCoroutine(MoveDisc(--k));
        }

        Disc disc = discs[index];
        Pole oldPole = disc.pole;
        if((discCount & 1) == 0) {
            if (oldPole.next.discs.Count == 0 || oldPole.next.discs.Peek().index > disc.index) {
                disc.pole = oldPole.next;

            } else {
                disc.pole = oldPole.next.next;

            }
        } else {
            if (oldPole.previous.discs.Count == 0 || oldPole.previous.discs.Peek().index > disc.index) {
                disc.pole = oldPole.previous;

            } else {
                disc.pole = oldPole.previous.previous;

            }
        }
        
        oldPole.discs.Pop();
        disc.pole.discs.Push(disc);
        yield return StartCoroutine(AnimateMove(disc, oldPole, disc.pole));
    }

    IEnumerator AnimateMove(Disc disc, Pole from, Pole to) {
        yield return new WaitForSeconds(1f/(1 + Mathf.Pow(3,discCount)/56));
        disc.transform.position = to.transform.position + to.discLocation + Vector3.up * (to.discs.Count - 1) * discHeight;
    }
}
