using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTPipetting.Hardware
{
    interface ILiha
    {
        //if move commands cannot be executed, throw exception
        void Init(); //move axis to home
        void Move2AbsolutePosition(float x, float xSpeed, float y, float ySpeed, float z, float zSpeed); // as name
        void MoveXAbs(float x,float speed);
        void MoveYAbs(float y, float speed);
        void MoveZAbs(float z, float speed);
        void MoveRelativeX(float x, float speed);
        void MoveRelativeY(float y, float speed);
        void MoveRelativeZ(float z, float speed);

        //if no tip fetched, throw cannotGet exception
        //if already mounted diti, throw alreadyMounted exception
        void GetTip();

        //return whether tips are on cone
        bool IsTipOn();

        //if tip cannot be dropped, throw cannotDrop exception
        //if no tip on diti cone, return immediately
        void DropTip();

        //throw exception if no liquid can be detected
        //throw exception if clot detected
        //return liquid detect level in mm
        //aspirationSpeed ul/s 
        //delay in ms, after aspiration, delay certain time span
        //leading air gap aspirate some air in ul before aspirate liquid
        //trailingAirgap aspirate some air in ul after aspirate liquid
        //excessVolume aspirate little more in ul
        float Aspirate(double volume, int speed, int delayMs, int leadingAirGap, int trailingAirGap, int excessVolume);
        void Dispense(double volume, int speed);
    }
}
