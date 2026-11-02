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