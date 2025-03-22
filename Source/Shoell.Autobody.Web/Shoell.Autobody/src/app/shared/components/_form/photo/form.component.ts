import { NgIf, NgStyle } from '@angular/common';
import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { DxFileUploaderModule } from 'devextreme-angular';

@Component({
    selector: 'form-photo',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, NgStyle, DxFileUploaderModule]
})
export class FormPhotoComponent implements OnInit {
  @Input() link!: string;

  @Input() editable = false;

  @Input() size = 124;

  imageUrl!: string;

  hostRef = this.elRef.nativeElement;

  constructor(private elRef: ElementRef) { }

  ngOnInit() {
    this.imageUrl = `url('${this.link}')`;
  }
}
