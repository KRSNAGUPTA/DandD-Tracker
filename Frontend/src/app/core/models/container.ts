export interface Container{
    id: number
    containerNo: string,
    bookingRef: string,
    type: string,
    destinationPort: string,
    status: string,
    daysElapsed: number,
    freeDaysRemaining: number,
    currentCharge: number,
    currency: string,
    risk: string
  
}
export interface Event {
  eventType: 'Discharged' | 'GatedOut' | 'EmptyReturned';
  occurredAt: string;
  note: string | null;
}

export interface Breakdown {
  fromDay: number;
  toDay: number | null;
  daysInTier: number;
  rate: number;
  subtotal: number;
}

export interface Charge {
  chargeType: 'Demurrage' | 'Detention';
  daysElapsed: number;
  freeDaysRemaining: number;
  amount: number;
  currency: string;
  breakdown: Breakdown[];
}

export interface ContainerDetail {
  id: number;
  containerNo: string;
  bookingRef: string;
  type: string;
  destinationPort: string;
  status: 'AtPort' | 'OutForDelivery' | 'Closed';
  createdAt: string;
  events: Event[];
  demurrage: Charge | null;
  detention: Charge | null;
}