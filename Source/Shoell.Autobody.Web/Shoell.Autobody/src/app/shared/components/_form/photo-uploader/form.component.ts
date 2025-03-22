import { NgClass } from '@angular/common';
import { Component } from '@angular/core';
import { DxFileUploaderModule } from 'devextreme-angular/ui/file-uploader';
import { DropZoneEnterEvent, DropZoneLeaveEvent } from 'devextreme/ui/file_uploader';

@Component({
    selector: 'form-photo-uploader',
    template: `
    <div 
      id="uploader"
      [ngClass]="
        isDropZoneActive
          ? ['dx-theme-accent-as-border-color']
          : ['dx-theme-border-color']
      "
    >
      <span>Drag and drop a photo here or click the area to select it from a folder</span>
    </div>
    <dx-file-uploader
      dialogTrigger="#uploader"
      dropZone="#uploader"
      [multiple]="false"
      accept="image/*"
      uploadMode="instantly"
      [showFileList]=false
      [visible]="false"
      (onDropZoneEnter)="onDropZoneEnter($event)"
      (onDropZoneLeave)="onDropZoneLeave($event)"
    >
    </dx-file-uploader>
`,
    styles: [`
    #uploader {
      background-color: rgba(183, 183, 183, 0.1);
      border-width: 2px;
      border-style: dashed;
      border-radius: 8px;
      padding: 10px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      height: 124px;
      user-select: none;
      line-height: inherit;
      margin-bottom: 10px;
      & > span {
        font-weight: 100;
        opacity: 0.5;
        text-align: center;
      }
    }
  `],
    imports: [NgClass, DxFileUploaderModule]
})
export class FormPhotoUploaderComponent {
  isDropZoneActive = false;

  onDropZoneEnter(e: DropZoneEnterEvent) {
    if (e.dropZoneElement.id === 'uploader') { this.isDropZoneActive = true; }
  }

  onDropZoneLeave(e: DropZoneLeaveEvent) {
    if (e.dropZoneElement.id === 'uploader') { this.isDropZoneActive = false; }
  }
}