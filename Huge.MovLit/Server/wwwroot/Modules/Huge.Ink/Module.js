window.inkEditor = (function(){
  const editors = new Map();

  function init(textarea, options){
    if (!window.CodeMirror || !textarea) return null;
    const cm = window.CodeMirror.fromTextArea(textarea, Object.assign({
      lineNumbers: true,
      mode: 'markdown',
      lineWrapping: true,
      theme: 'default'
    }, options || {}));
    editors.set(textarea, cm);
    return true;
  }

  function getValue(textarea){
    const cm = editors.get(textarea);
    return cm ? cm.getValue() : textarea?.value;
  }

  function setValue(textarea, value){
    const cm = editors.get(textarea);
    if (cm) cm.setValue(value ?? '');
  }

  function dispose(textarea){
    const cm = editors.get(textarea);
    if (cm){
      cm.toTextArea();
      editors.delete(textarea);
    }
  }

  return { init, getValue, setValue, dispose };
})();
