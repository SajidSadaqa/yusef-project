"use client"

import { useRef } from "react"
import { Canvas, useFrame } from "@react-three/fiber"
import { Text3D, Center } from "@react-three/drei"
import { useScroll } from "framer-motion"
import type * as THREE from "three"

function Logo3DModel() {
  const logoRef = useRef<THREE.Group>(null)
  const { scrollYProgress } = useScroll()

  useFrame(() => {
    if (logoRef.current) {
      const scrollValue = scrollYProgress.get()
      // Rotate and move logo based on scroll
      logoRef.current.rotation.y = scrollValue * Math.PI * 2
      logoRef.current.position.y = Math.sin(scrollValue * Math.PI * 4) * 0.5
      logoRef.current.position.z = -2 + scrollValue * 2
    }
  })

  return (
    <group ref={logoRef}>
      <Center>
        {/* V Letter in 3D */}
        <group position={[-2, 0, 0]}>
          <Text3D
            font="/fonts/Inter_Bold.json"
            size={1.2}
            height={0.3}
            curveSegments={12}
            bevelEnabled
            bevelThickness={0.05}
            bevelSize={0.02}
            bevelOffset={0}
            bevelSegments={5}
          >
            V
            <meshStandardMaterial color="#dc2626" metalness={0.7} roughness={0.3} />
          </Text3D>
        </group>

        {/* ERTEX Text in 3D */}
        <group position={[0, 0, 0]}>
          <Text3D
            font="/fonts/Inter_Bold.json"
            size={0.8}
            height={0.25}
            curveSegments={12}
            bevelEnabled
            bevelThickness={0.04}
            bevelSize={0.02}
            bevelOffset={0}
            bevelSegments={5}
          >
            ERTEX
            <meshStandardMaterial color="#2563eb" metalness={0.6} roughness={0.4} />
          </Text3D>
        </group>

        {/* Subtle glow effect */}
        <pointLight position={[0, 0, 2]} intensity={0.5} color="#dc2626" />
        <pointLight position={[0, 0, -2]} intensity={0.3} color="#2563eb" />
      </Center>
    </group>
  )
}

export function Logo3D() {
  return (
    <div className="fixed top-20 right-8 w-64 h-32 pointer-events-none z-10 opacity-30">
      <Canvas camera={{ position: [0, 0, 8], fov: 45 }} style={{ background: "transparent" }}>
        <ambientLight intensity={0.5} />
        <directionalLight position={[5, 5, 5]} intensity={1} />
        <directionalLight position={[-5, -5, -5]} intensity={0.3} />
        <Logo3DModel />
      </Canvas>
    </div>
  )
}
