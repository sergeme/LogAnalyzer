export default class LogEntry {
  id: number;
  clientIP: string;
  clientFQDN: string;
  visits: number;

  constructor(id: number, clientIP: string, clientFQDN: string, visits: number) {
    this.id = id;
    this.clientIP = clientIP;
    this.clientFQDN = clientFQDN;
    this.visits = visits;
  }
}