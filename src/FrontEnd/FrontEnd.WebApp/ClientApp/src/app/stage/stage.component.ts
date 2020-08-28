import { Component, OnInit } from '@angular/core';
import { StageControllerService } from '../services/stage-controller.service';

@Component({
  selector: 'app-stage',
  templateUrl: './stage.component.html',
  styleUrls: ['./stage.component.css']
})
export class StageComponent implements OnInit {

  constructor(private stageController: StageControllerService) { }

  ngOnInit(): void {
  }

}
