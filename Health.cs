using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    //현재 체력
    public int health;
    //최대체력
    public int numOfHearts;

    //하트 이미지들
    public Image[] hearts;
    //꽉찬 하트
    public Sprite fullHeart;
    //반 하트
    public Sprite halfHeart;
    //빈 하트
    public Sprite emptyHeart;

    private void Update()
    {
        if(health > numOfHearts)
        {
            health = numOfHearts;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health) // 4
            {
                hearts[i].sprite = fullHeart;
            }
            else if(i >= health - 1)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
