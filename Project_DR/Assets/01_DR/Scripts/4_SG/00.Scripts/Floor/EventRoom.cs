using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRoom : RandomRoom
{       // EventRoomClass는 스프레드시트의 값을 받아와서 랜덤한 이벤트 발생을 시켜줄 Class

    
    

    void Start()
    {
       DungeonManager.clearList.Add(isClearRoom);
    }       // Start()

    private void OnDestroy()
    {
        DungeonManager.clearList.Remove(isClearRoom);
    }       // OnDestroy()




}       // ClassEnd
