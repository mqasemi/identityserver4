import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-call-api',
  templateUrl: './call-api.component.html',
  styleUrls: ['./call-api.component.css']
})
export class CallApiComponent implements OnInit {
  response: Object;
  constructor(private http: HttpClient,
    private authService: AuthService) { }
   
  ngOnInit(): void {
    
    let headers = new HttpHeaders({ 'Authorization': this.authService.getAuthorizationHeaderValue() });
    this.http.get("http://localhost:5001/identity", { headers: headers })
          .subscribe(response => this.response = response);
  }

}
