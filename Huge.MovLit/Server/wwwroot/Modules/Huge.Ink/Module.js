if (window.CodeMirror && window.CodeMirror.defineSimpleMode && !window.CodeMirror.modes?.ink) {
    window.CodeMirror.defineSimpleMode('ink', {
        start: [
            { regex: /^\s*===.*===\s*$/, token: 'header' },           // knots/stitches
            { regex: /^\s*~.*/, token: 'def' },         // script lines
            { regex: /^\s*VAR\b.*$/, token: 'keyword' },     // VAR declarations
            { regex: /^\s*(\*|\+).*/, token: 'atom' },        // choices (* or +)
            { regex: /->\s*[\w\.\-]+/, token: 'link' },        // divert
            { regex: /^\s*#.*$/, token: 'tag' },         // tags (line-level)
            { regex: /#\w[\w\-]*/, token: 'tag' }          // inline #tags
        ],
        meta: { lineComment: '//', fold: 'brace' }
    });
}

window.inkEditor = (function () {
  const editors = new Map();

  function init(textarea, options){
    if (!window.CodeMirror || !textarea) return null;
    const cm = window.CodeMirror.fromTextArea(textarea, Object.assign({
      lineNumbers: true,
      mode: 'ink',
      lineWrapping: true,
      theme: 'default',
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
      const id = textarea && textarea.id;
      if (id && editorsById.get(id) === cm) editorsById.delete(id);
    }
  }

  // Dispose by id (for cases where the element reference changed)
  function disposeById(id){
    if (!id) return;
    const cm = editorsById.get(id);
    if (cm){
      try { cm.toTextArea(); } catch {}
      editorsById.delete(id);
    }
  }

  return { init, getValue, setValue, dispose, disposeById };
})();
