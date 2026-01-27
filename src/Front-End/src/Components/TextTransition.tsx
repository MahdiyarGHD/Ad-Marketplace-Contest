import { memo, useEffect, useRef, useState } from "react";
import Transition from "./Transition";

function TextTransition({
	text,
	style,
}: {
	text: string;
	style?: React.CSSProperties;
}) {
	// const [prevText, setPrevText] = useState()
	// const [currentText, setCurrentText] = useState()
	const [animating, setAnimating] = useState(false);

	const prevText = useRef("");

	useEffect(() => {
		// setCurrentText(text)

		return () => {
			// setPrevText(text)
			prevText.current = text;
			setAnimating(true);
		};
	}, [text]);

	return (
		<div className="TextTransition" style={style}>
			{text && (
				<Transition state={true} key={text} alwaysShow>
					<span className="">{text}</span>
				</Transition>
			)}
			{animating && (
				<Transition
					state={true}
					alwaysShow
					activeAction={() => setTimeout(() => setAnimating(false), 300)}
				>
					<span className="prev">{prevText.current}</span>
				</Transition>
			)}
		</div>
	);
}

export default memo(TextTransition);
