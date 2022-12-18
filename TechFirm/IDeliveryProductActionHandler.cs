using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechFirm.Models;

namespace TechFirm
{
    public interface IDeliveryProductActionHandler
    {
        void OnAddAction(DeliveryProduct deliveryProduct);

        void OnEditAction(int index, DeliveryProduct deliveryProduct);
    }
}
